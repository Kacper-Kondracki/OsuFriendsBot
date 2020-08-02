using Discord;
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
        private readonly DbUserDataService _dbUserData;
        private readonly DiscordSocketClient _discord;
        private readonly OsuFriendsClient _osuFriends;
        private readonly ILogger _logger;

        private readonly Dictionary<ulong, ulong> verifyingUsers = new Dictionary<ulong, ulong>();
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
            _ = VerifyAsync(user);
            await Task.CompletedTask;
        }

        public async Task VerifyAsync(SocketGuildUser user)
        {
            try
            {
                bool isVeryfying = false;
                lock (verifyingUsersLock)
                {
                    if (verifyingUsers.TryGetValue(user.Id, out ulong guild))
                    {
                        isVeryfying = guild == user.Guild.Id;
                    }
                    if (!isVeryfying)
                    {
                        verifyingUsers[user.Id] = user.Guild.Id;
                    }
                }
                if (isVeryfying)
                {
                    await user.SendMessageAsync("Complete your first verification before starting next one!");
                    return;
                }

                UserData dbUser = _dbUserData.FindById(user.Id);
                _logger.LogDebug("dbUser : {@dbUser}\n Id : {@user}\nUsername: {@username}", dbUser, user.Id, user.Username);

                EmbedBuilder embedBuilder;
                OsuUser osuUser;

                if (dbUser == null)
                {
                    // If user doesn't exist in db
                    while (true)
                    {
                        osuUser = _osuFriends.CreateUser();
                        if ((await osuUser.GetStatusAsync()) == null)
                        {
                            break;
                        }
                    }

                    embedBuilder = new EmbedBuilder();
                    embedBuilder
                        .WithTitle($"Hi {user.Username}!")
                        .WithDescription($"Verify your osu! account to get cool roles on {user.Guild.Name}!")
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
                    // Verification Success
                    _dbUserData.Upsert(new UserData { UserId = user.Id, OsuFriendsKey = osuUser.Key });
                }
                else
                {
                    // If user exist in db
                    osuUser = _osuFriends.CreateUser(dbUser.OsuFriendsKey);
                    if (await osuUser.GetStatusAsync() != Status.Completed)
                    {
                        await user.SendMessageAsync($"Refreshing failed! Refresh your account again with 'refresh' command on {user.Guild.Name}");
                        return;
                    }
                    // Refresh Success
                }
                // Success for both
                OsuUserDetails osuUserDetails = await osuUser.GetDetailsAsync();

                IReadOnlyCollection<SocketRole> guildRoles = user.Guild.Roles;

                List<SocketRole> roles = FindUserRoles(guildRoles, osuUserDetails);

                await user.RemoveRolesAsync(FindAllRoles(guildRoles).Where(role => user.Roles.Contains(role) && !roles.Contains(role)));
                await user.AddRolesAsync(roles.Where(role => !user.Roles.Contains(role)));

                embedBuilder = new EmbedBuilder();
                embedBuilder
                    .WithTitle($"Granted roles on {user.Guild.Name}:")
                    .WithDescription(string.Join('\n', roles.Select(role => role.Name)))
                    .WithThumbnailUrl(osuUserDetails.Avatar.ToString());
                await user.SendMessageAsync(embed: embedBuilder.Build());
            }
            finally
            {
                lock (verifyingUsersLock)
                {
                    verifyingUsers.Remove(user.Id);
                }
            }
        }

        public static string DigitRole(int digit, Gamemode gamemode)
        {
            return gamemode switch
            {
                Gamemode.Std => $"[STD] {digit} DIGIT",
                Gamemode.Taiko => $"[TAIKO] {digit} DIGIT",
                Gamemode.Ctb => $"[CTB] {digit} DIGIT",
                Gamemode.Mania => $"[MANIA] {digit} DIGIT",
                Gamemode.Generic => $"{digit} DIGIT",
                _ => throw new NotImplementedException(),
            };
        }

        public static string PlaystyleRole(Playstyle playstyle)
        {
            return playstyle.ToString().ToUpperInvariant();
        }

        public static SocketRole FindDigitRole(IReadOnlyCollection<SocketRole> roles, int digit, Gamemode gamemode)
        {
            return roles.FirstOrDefault(role => role.Name.Equals(DigitRole(digit, gamemode), StringComparison.InvariantCultureIgnoreCase));
        }

        public static List<SocketRole> FindPlaystyleRoles(IReadOnlyCollection<SocketRole> roles, List<Playstyle> playstyles)
        {
            IEnumerable<string> playstylesString = playstyles.Select(playstyle => PlaystyleRole(playstyle));
            return roles.Where(role => playstylesString.Contains(role.Name, StringComparer.InvariantCultureIgnoreCase)).ToList();
        }

        public static List<SocketRole> FindUserRoles(IReadOnlyCollection<SocketRole> roles, OsuUserDetails osuUserDetails)
        {
            List<SocketRole> allRoles = FindPlaystyleRoles(roles, osuUserDetails.Playstyle);
            if (osuUserDetails.Std != null)
            {
                int std = osuUserDetails.Std.ToString().Length;
                SocketRole digitRole = FindDigitRole(roles, std, Gamemode.Std);
                allRoles.Add(digitRole);

                digitRole = FindDigitRole(roles, std, Gamemode.Generic);
                allRoles.Add(digitRole);
            }
            if (osuUserDetails.Taiko != null)
            {
                int taiko = osuUserDetails.Taiko.ToString().Length;
                SocketRole digitRole = FindDigitRole(roles, taiko, Gamemode.Taiko);
                allRoles.Add(digitRole);

                digitRole = FindDigitRole(roles, taiko, Gamemode.Generic);
                allRoles.Add(digitRole);
            }
            if (osuUserDetails.Ctb != null)
            {
                int ctb = osuUserDetails.Ctb.ToString().Length;
                SocketRole digitRole = FindDigitRole(roles, ctb, Gamemode.Ctb);
                allRoles.Add(digitRole);

                digitRole = FindDigitRole(roles, ctb, Gamemode.Generic);
                allRoles.Add(digitRole);
            }
            if (osuUserDetails.Mania != null)
            {
                int mania = osuUserDetails.Mania.ToString().Length;
                SocketRole digitRole = FindDigitRole(roles, mania, Gamemode.Mania);
                allRoles.Add(digitRole);

                digitRole = FindDigitRole(roles, mania, Gamemode.Generic);
                allRoles.Add(digitRole);
            }
            return allRoles.Distinct().Where(role => role != null).ToList();
        }

        public static List<string> AllRoles()
        {
            List<string> allRoles = new List<string>();
            foreach (Playstyle playstyle in Enum.GetValues(typeof(Playstyle)))
            {
                allRoles.Add(PlaystyleRole(playstyle));
            }
            foreach (Gamemode gamemode in Enum.GetValues(typeof(Gamemode)))
            {
                for (int i = 1; i <= 7; i++)
                {
                    allRoles.Add(DigitRole(i, gamemode));
                }
            }
            return allRoles;
        }

        public static List<SocketRole> FindAllRoles(IReadOnlyCollection<SocketRole> roles)
        {
            List<SocketRole> allRoles = FindPlaystyleRoles(roles, new List<Playstyle>() { Playstyle.Keyboard, Playstyle.Mouse, Playstyle.Tablet, Playstyle.Touchscreen });
            foreach (Gamemode gamemode in Enum.GetValues(typeof(Gamemode)))
            {
                for (int i = 1; i <= 7; i++)
                {
                    SocketRole digitRole = FindDigitRole(roles, i, gamemode);
                    if (digitRole != null)
                    {
                        allRoles.Add(digitRole);
                    }
                }
            }
            return allRoles;
        }
    }

    public enum Gamemode
    {
        Std,
        Taiko,
        Ctb,
        Mania,
        Generic
    }
}