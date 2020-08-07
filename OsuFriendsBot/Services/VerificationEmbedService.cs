using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using OsuFriendsApi.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OsuFriendsBot.Services
{
    public class VerificationEmbedService
    {
        private readonly ILogger<VerificationEmbedService> _logger;

        public VerificationEmbedService(ILogger<VerificationEmbedService> logger)
        {
            _logger = logger;
        }

        public EmbedBuilder CreateVerifyEmbed(SocketGuildUser user, OsuUser osuUser)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder
                .WithTitle($"Hi {user.Username}!")
                .WithDescription($"Verify your osu! account to get cool roles on {user.Guild.Name}!")
                .AddField("Link", osuUser.Url)
                .WithThumbnailUrl("https://osufriends.ovh/img/favicon.gif");

            return embedBuilder;
        }

        public EmbedBuilder CreateGrantedRolesEmbed(SocketGuildUser user, List<SocketRole> grantedRoles, OsuUserDetails osuUserDetails)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder = new EmbedBuilder();
            embedBuilder
                .WithTitle($"Granted roles on {user.Guild.Name}:")
                .WithDescription(string.Join('\n', grantedRoles.Select(role => role.Name)))
                .WithThumbnailUrl(osuUserDetails.Avatar.ToString());

            return embedBuilder;
        }
    }
}