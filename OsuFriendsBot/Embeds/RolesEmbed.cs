using Discord;
using System.Collections.Generic;
using System.Linq;

namespace OsuFriendsBot.Embeds
{
    public class RolesEmbed : EmbedBuilder
    {
        public RolesEmbed(string title, List<string> roles)
        {
            Title = title;
            Color = EmbedColors.Info;

            if (roles.Any())
            {
                Description = string.Join('\n', roles);
            }
            else
            {
                Description = "None";
            }
        }
    }
}