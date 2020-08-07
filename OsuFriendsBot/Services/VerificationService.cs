using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using OsuFriendsApi;
using OsuFriendsApi.Entities;
using OsuFriendsDb.Models;
using OsuFriendsDb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OsuFriendsBot.Services
{
    public class VerificationService
    {
        private readonly OsuRoleFindingService _osuRoles;
        private readonly VerificationEmbedService _embed;
        private readonly DbUserDataService _dbUserData;
        private readonly DiscordSocketClient _discord;
        private readonly OsuFriendsClient _osuFriends;
        private readonly ILogger _logger;

        private readonly HashSet<ulong> verifyingUsers = new HashSet<ulong>();
        private static readonly object verifyingUsersLock = new object();

        public VerificationService(OsuRoleFindingService osuRoles, VerificationEmbedService embed, DbUserDataService dbUserData, DiscordSocketClient discord, OsuFriendsClient osuFriends, ILogger<VerificationService> logger)
        {
            _osuRoles = osuRoles;
            _embed = embed;
            _dbUserData = dbUserData;
            _discord = discord;
            _osuFriends = osuFriends;
            _logger = logger;

            _discord.UserJoined += UserJoinedAsync;
        }

        public async Task UserJoinedAsync(SocketGuildUser user)
        {
            _ = VerifyAsync(user);
            await Task.CompletedTask;
        }

        public async Task VerifyAsync(SocketGuildUser user)
        {
            try
            {
                bool isVeryfying = AddVerifyingUser(user);
                if (isVeryfying)
                {
                    await user.SendMessageAsync("Complete your first verification before starting next one!"); // Should be error
                    return;
                }

                UserData dbUser = _dbUserData.FindById(user.Id);
                _logger.LogDebug("dbUser : {@dbUser}\n Id : {@user}\nUsername: {@username}", dbUser, user.Id, user.Username);

                OsuUser osuUser;
                if (dbUser == null)
                {
                    // If user doesn't exist in db
                    osuUser = await CreateOsuUserAsync();
                    await user.SendMessageAsync(embed: _embed.CreateVerifyEmbed(user, osuUser).Build());

                    // Retry
                    bool success = await WaitForVerificationStatusAsync(osuUser);
                    if (!success)
                    {
                        await user.SendMessageAsync($"Verification failed! Verify your account again with 'verify' command on {user.Guild.Name}"); // Should be error
                        return;
                    }
                    // Verification Success
                    _dbUserData.Upsert(new UserData { UserId = user.Id, OsuFriendsKey = osuUser.Key });
                }
                else
                {
                    osuUser = await CreateOsuUserFromUserDataAsync(dbUser);
                    if (osuUser == null)
                    {
                        await user.SendMessageAsync($"Verification failed! Verify your account again with 'verify' command on {user.Guild.Name}"); // Should be error
                        return;
                    }
                }
                // Success for both
                (List<SocketRole> grantedRoles, OsuUserDetails osuUserDetails) = await GrantUserRolesAsync(user, osuUser);
                await user.SendMessageAsync(embed: _embed.CreateGrantedRolesEmbed(user, grantedRoles, osuUserDetails).Build());
            }
            finally
            {
                RemoveVerifyingUser(user);
            }
        }

        private bool AddVerifyingUser(SocketGuildUser user)
        {
            lock (verifyingUsersLock)
            {
                bool isVerifying = verifyingUsers.Contains(user.Id);
                if (!isVerifying)
                {
                    verifyingUsers.Add(user.Id);
                }
                return isVerifying;
            }
        }

        private void RemoveVerifyingUser(SocketGuildUser user)
        {
            lock (verifyingUsersLock)
            {
                verifyingUsers.Remove(user.Id);
            }
        }

        private async Task<OsuUser> CreateOsuUserAsync()
        {
            while (true)
            {
                OsuUser osuUser = _osuFriends.CreateUser();
                if ((await osuUser.GetStatusAsync()) == null)
                {
                    return osuUser;
                }
            }
        }

        private async Task<OsuUser> CreateOsuUserFromUserDataAsync(UserData userData)
        {
            OsuUser osuUser = _osuFriends.CreateUser(userData.OsuFriendsKey);
            if (await osuUser.GetStatusAsync() != Status.Completed)
            {
                return null;
            }
            return osuUser;
        }

        private async Task<bool> WaitForVerificationStatusAsync(OsuUser osuUser)
        {
            bool success = false;
            for (int retry = 0; retry < 20; retry++)
            {
                _logger.LogDebug("Status {status}", await osuUser.GetStatusAsync());
                if (await osuUser.GetStatusAsync() == Status.Completed)
                {
                    success = true;
                    break;
                }
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
            return success;
        }

        private async Task<(List<SocketRole>, OsuUserDetails)> GrantUserRolesAsync(SocketGuildUser user, OsuUser osuUser)
        {
            OsuUserDetails osuUserDetails = await osuUser.GetDetailsAsync();
            IReadOnlyCollection<SocketRole> guildRoles = user.Guild.Roles;
            // Find roles that user should have
            List<SocketRole> roles = _osuRoles.FindUserRoles(guildRoles, osuUserDetails);
            // Remove roles that user shouldn't have
            await user.RemoveRolesAsync(_osuRoles.FindAllRoles(guildRoles).Where(role => user.Roles.Contains(role) && !roles.Contains(role)));
            // Add roles that user should have
            await user.AddRolesAsync(roles.Where(role => !user.Roles.Contains(role)));
            // Change user nickname to that from game

            // Ignore if can't change nickname
            try
            {
                await user.ModifyAsync(properties => properties.Nickname = osuUserDetails.Username);
            }
            catch (HttpException) { }
            return (roles, osuUserDetails);
        }
    }
}