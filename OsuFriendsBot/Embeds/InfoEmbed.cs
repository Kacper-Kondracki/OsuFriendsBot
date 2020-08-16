using Discord;
using Discord.Rest;

namespace OsuFriendsBot.Embeds
{
    public class InfoEmbed : EmbedBuilder
    {
        public InfoEmbed(RestApplication app)
        {
            Title = $"About {app.Name}";
            Description = app.Description;
            ThumbnailUrl = app.IconUrl;
            AddField("Author:", app.Owner);
            AddField("Git repo:", @"https://github.com/AbdShullah/OsuFriendsBot");
            AddField("Bot Version:", "0.0.8");
            Color = EmbedColors.Info;
        }
    }
}