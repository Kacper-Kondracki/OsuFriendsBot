using Discord;
using System.Collections.Generic;
using System.Linq;

namespace OsuFriendsBot.Embeds
{
    public class RolesEmbed : EmbedBuilder
    {
        public RolesEmbed(RoleEmbedType type, List<string> roles)
        {
            Title = type switch
            {
                RoleEmbedType.Created => "Created Roles:",
                RoleEmbedType.Configured => "Configured Roles:",
                RoleEmbedType.Missing => "Missing Roles:",
                RoleEmbedType.Deleted => "Deleted Roles:",
                _ => throw new System.NotImplementedException()
            };

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

    public enum RoleEmbedType
    {
        Created,
        Configured,
        Missing,
        Deleted
    }
}