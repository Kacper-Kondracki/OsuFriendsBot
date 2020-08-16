using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using OsuFriendsApi;
using OsuFriendsApi.Entities;
using OsuFriendsBot.Embeds;
using OsuFriendsBot.Osu.OsuFriendsBot.Services;
using OsuFriendsBot.RuntimeResults;
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
        private readonly DbUserDataService _dbUserData;
        private readonly DiscordSocketClient _discord;
        private readonly OsuFriendsClient _osuFriends;
        private readonly ILogger _logger;

        private readonly HashSet<ulong> verifyingUsers = new HashSet<ulong>();
        private static readonly object verifyingUsersLock = new object();

        public VerificationService(DbUserDataService dbUserData, DiscordSocketClient discord, OsuFriendsClient osuFriends, ILogger<VerificationService> logger)
        {
            _dbUserData = dbUserData;
            _discord = discord;
            _osuFriends = osuFriends;
            _logger = logger;

            _discord.UserJoined += UserJoinedAsync;
        }

        public async Task UserJoinedAsync(SocketGuildUser user)
        {
            _ = Task.Run(async () =>
            {
                RuntimeResult result = await VerifyAsync(user);

                // Send error if can open DM
                if (!result.IsSuccess)
                {
                    try
                    {
                        await user.SendMessageAsync(embed: new ErrorEmbed(result).Build());
                    }
                    catch (HttpException e)
                    {
                        switch (e.DiscordCode)
                        {
                            case 50007:
                                return;

                            default:
                                break;
                        }
                        throw;
                    }
                }
            });
            await Task.CompletedTask;
        }

        public async Task<RuntimeResult> VerifyAsync(SocketGuildUser user, SocketCommandContext context = null)
        {
            _logger.LogTrace("Verifying user: {username}", user.Username);

            try
            {
                bool isVeryfying = AddVerifyingUser(user);
                if (isVeryfying)
                {
                    return VerificationResult.FromError("Complete your first verification before starting next one");
                }
                UserData dbUser = _dbUserData.FindById(user.Id);
                _logger.LogTrace("DbUser : {@dbUser} | Id : {@user} | Username: {@username}", dbUser, user.Id, user.Username);

                OsuUser osuUser = null;

                if (dbUser.OsuFriendsKey != null)
                {
                    osuUser = await CreateOsuUserFromUserDataAsync(dbUser);
                }

                if (osuUser == null)
                {
                    // If user doesn't exist in db
                    osuUser = await CreateOsuUserAsync();
                    await user.SendMessageAsync(embed: new VerifyEmbed(user, osuUser).Build());

                    // Retry
                    bool success = await WaitForVerificationStatusAsync(osuUser);
                    if (!success)
                    {
                        RemoveVerifyingUser(user);
                        return VerificationResult.FromError($"Verification failed because it timeouted! Try again with 'verify' command on {user.Guild.Name}");
                    }
                    // Verification Success
                    dbUser.OsuFriendsKey = osuUser.Key;
                }
                // Success for both
                (List<SocketRole> grantedRoles, OsuUserDetails osuUserDetails) = await GrantUserRolesAsync(user, osuUser);
                await SendEmbedMsg(user, context, embed: new GrantedRolesEmbed(user, grantedRoles, osuUserDetails, dbUser).Build());
                dbUser.Std = osuUserDetails.Std;
                dbUser.Taiko = osuUserDetails.Taiko;
                dbUser.Ctb = osuUserDetails.Ctb;
                dbUser.Mania = osuUserDetails.Mania;
                _dbUserData.Upsert(dbUser);
            }
            catch (HttpException e)
            {
                RemoveVerifyingUser(user);
                _logger.LogTrace("httpCode: {httpCode} | discordCode: {discordCode}", e.HttpCode, e.DiscordCode);
                switch (e.DiscordCode)
                {
                    case 50007:
                        return VerificationResult.FromError("Sorry, I can't send direct message to you, please check if you block DMs via server");

                    default:
                        break;
                }
                throw;
            }
            catch
            {
                RemoveVerifyingUser(user);
                throw;
            }
            RemoveVerifyingUser(user);
            return VerificationResult.FromSuccess();
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
                Status? status = await osuUser.GetStatusAsync();
                _logger.LogTrace("Verification Status: {status}", status);
                if (status == Status.Invalid)
                {
                    return osuUser;
                }
            }
        }

        private async Task<OsuUser> CreateOsuUserFromUserDataAsync(UserData userData)
        {
            OsuUser osuUser = _osuFriends.CreateUser(userData.OsuFriendsKey);
            Status? status = await osuUser.GetStatusAsync();
            _logger.LogTrace("OsuDb Status: {status}", status);
            if (status != Status.Completed)
            {
                return null;
            }
            return osuUser;
        }

        private async Task<bool> WaitForVerificationStatusAsync(OsuUser osuUser)
        {
            bool success = false;
            for (int retry = 0; retry < 60; retry++)
            {
                Status? status = await osuUser.GetStatusAsync();
                _logger.LogTrace("Verification Status: {@status}", status);
                if (status == Status.Completed)
                {
                    success = true;
                    break;
                }
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
            return success;
        }

        private async Task SendEmbedMsg(SocketGuildUser user, SocketCommandContext context, Embed embed)
        {
            if (context != null)
            {
                await context.Channel.SendMessageAsync(embed: embed);
            }
            else
            {
                await user.SendMessageAsync(embed: embed);
            }
        }

        private async Task<(List<SocketRole>, OsuUserDetails)> GrantUserRolesAsync(SocketGuildUser user, OsuUser osuUser)
        {
            OsuUserDetails osuUserDetails = await osuUser.GetDetailsAsync();
            _logger.LogTrace("Details: {@details}", osuUserDetails);
            IReadOnlyCollection<SocketRole> guildRoles = user.Guild.Roles;
            // Find roles that user should have
            List<SocketRole> roles = OsuRoles.FindUserRoles(guildRoles, osuUserDetails);
            // Remove roles that user shouldn't have
            await user.RemoveRolesAsync(OsuRoles.FindAllRoles(guildRoles).Where(role => user.Roles.Contains(role) && !roles.Contains(role)));
            // Add roles that user should have
            await user.AddRolesAsync(roles.Where(role => !user.Roles.Contains(role)));
            // Change user nickname to that from game

            // Ignore if can't change nickname
            try
            {
                await user.ModifyAsync(properties => properties.Nickname = osuUserDetails.Username);
            }
            catch (HttpException)
            {
            }

            return (roles, osuUserDetails);
        }
    }
}