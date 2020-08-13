using Discord;
using Discord.WebSocket;
using OsuFriendsApi.Entities;

namespace OsuFriendsBot.Embeds
{
    public class VerifyEmbed : EmbedBuilder
    {
        public VerifyEmbed(SocketGuildUser user, OsuUser osuUser)
        {
            Title = $"Hi {user.Username}!";
            Description = $"Verify your osu! account to get cool roles on {user.Guild.Name}!";
            AddField("Link", osuUser.Url);
            ThumbnailUrl = "https://osufriends.ovh/img/favicon.gif";
            Color = EmbedColors.Important;
        }
    }
}