using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using OsuFriendsApi;
using OsuFriendsApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OsuFriendsBot.Services
{
    public class VerificationService
    {
        private readonly DiscordSocketClient _discord;
        private readonly OsuFriendsClient _osuFriends;
        private readonly ILogger _logger;

        private readonly HashSet<ulong> verifyingUsers = new HashSet<ulong>();
        private static readonly object verifyingUsersLock = new object();

        public VerificationService(DiscordSocketClient discord, OsuFriendsClient osuFriends, ILogger<VerificationService> logger)
        {
            _discord = discord;
            _osuFriends = osuFriends;
            _logger = logger;

            _discord.UserJoined += UserJoinedAsync;
        }

        public async Task UserJoinedAsync(SocketGuildUser user)
        {
            _ = Verify(user);
            await Task.CompletedTask;
        }

        public async Task Verify(SocketGuildUser user)
        {
            // TODO: Check database before sending message

            bool isVeryfying = false;
            lock (verifyingUsersLock)
            {
                isVeryfying = verifyingUsers.Contains(user.Id);
                if (!isVeryfying)
                {
                    verifyingUsers.Add(user.Id);
                }
            }
            if (isVeryfying)
            {
                await user.SendMessageAsync("Complete your first verification before starting next one!");
                return;
            }

            OsuUser osuUser = null;
            while (true)
            {
                osuUser = _osuFriends.CreateUser();
                if ((await osuUser.GetStatusAsync()) == null)
                {
                    break;
                }
            }

            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder
                .WithTitle($"Hi {user.Username}!")
                .WithDescription($"Verify your osu! account to get cool roles!")
                .AddField("Link", osuUser.Url)
                .WithThumbnailUrl("https://osufriends.ovh/img/favicon.gif");

            await user.SendMessageAsync(embed: embedBuilder.Build());

            // Retry
            bool success = false;
            for (int retry = 0; retry < 20; retry++)
            {
                if (await osuUser.GetStatusAsync() == Status.Completed)
                {
                    success = true;
                    break;
                }
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
            if (!success)
            {
                await user.SendMessageAsync($"Verification failed! Verify your account again with 'verify' command on {user.Guild.Name}");
                return;
            }

            // Success
            OsuUserDetails osuUserDetails = await osuUser.GetDetailsAsync();

            IReadOnlyCollection<SocketRole> guildRoles = user.Guild.Roles;

            List<SocketRole> roles = FindUserRoles(guildRoles, osuUserDetails);
            await user.RemoveRolesAsync(FindAllRoles(guildRoles).Where(role => user.Roles.Contains(role) && !roles.Contains(role)));
            await user.AddRolesAsync(roles.Where(role => !user.Roles.Contains(role)));

            embedBuilder = new EmbedBuilder();
            embedBuilder
                .WithTitle("Granted roles:")
                .WithDescription(string.Join('\n', roles.Select(role => role.Name)))
                .WithThumbnailUrl(osuUserDetails.Avatar.ToString());
            await user.SendMessageAsync(embed: embedBuilder.Build());

            lock (verifyingUsersLock)
            {
                verifyingUsers.Remove(user.Id);
            }
        }

        public static SocketRole FindDigitRole(IReadOnlyCollection<SocketRole> roles, int digit)
        {
            return roles.FirstOrDefault(role => role.Name.Contains($"{digit} DIGIT", StringComparison.InvariantCultureIgnoreCase));
        }

        public static List<SocketRole> FindPlaystyleRoles(IReadOnlyCollection<SocketRole> roles, List<Playstyle> playstyles)
        {
            IEnumerable<string> playstylesString = playstyles.Select(playstyle => playstyle.ToString());
            return roles.Where(role => playstylesString.Contains(role.Name, StringComparer.InvariantCultureIgnoreCase)).ToList();
        }

        public static List<SocketRole> FindUserRoles(IReadOnlyCollection<SocketRole> roles, OsuUserDetails osuUserDetails)
        {
            List<SocketRole> allRoles = FindPlaystyleRoles(roles, osuUserDetails.Playstyle);
            SocketRole digitRole = FindDigitRole(roles, osuUserDetails.Level.Rank.Global.ToString().Length);
            if (digitRole != null)
            {
                allRoles.Add(digitRole);
            }
            return allRoles;
        }

        public static List<SocketRole> FindAllRoles(IReadOnlyCollection<SocketRole> roles)
        {
            List<SocketRole> allRoles = FindPlaystyleRoles(roles, new List<Playstyle>() { Playstyle.Keyboard, Playstyle.Mouse, Playstyle.Tablet, Playstyle.Touchscreen });
            for (int i = 1; i <= 7; i++)
            {
                SocketRole digitRole = FindDigitRole(roles, i);
                if (digitRole != null)
                {
                    allRoles.Add(digitRole);
                }
            }
            return allRoles;
        }
    }
}