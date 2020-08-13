using Discord;
using Discord.Commands;

namespace OsuFriendsBot.Embeds
{
    public class ErrorEmbed : EmbedBuilder
    {
        public ErrorEmbed(IResult result)
        {
            Title = "Error";
            Description = result.ErrorReason;
            ThumbnailUrl = @"https://anime-girls-holding-programming-books.netlify.app/static/Megumin_Holding_C_Programming_Language-5160545f66be5dfd903ba7f23bde63c9.jpg";
            Color = EmbedColors.Error;
        }
    }
}