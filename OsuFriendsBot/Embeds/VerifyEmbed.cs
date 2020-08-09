using Discord;
using Discord.WebSocket;
using OsuFriendsApi.Entities;

namespace OsuFriendsBot.Embeds
{
    public class VerifyEmbed : EmbedBuilder
    {
        public VerifyEmbed(SocketGuildUser user, OsuUser osuUser)
        {
            WithTitle($"Hi {user.Username}!")
            .WithDescription($"Verify your osu! account to get cool roles on {user.Guild.Name}!")
            .AddField("Link", osuUser.Url)
            .WithThumbnailUrl("https://osufriends.ovh/img/favicon.gif");
        }
    }
}