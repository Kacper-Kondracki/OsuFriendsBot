using Discord;
using Discord.WebSocket;
using OsuFriendsApi.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OsuFriendsBot.Embeds
{
    public class GrantedRolesEmbed : EmbedBuilder
    {
        public GrantedRolesEmbed(SocketGuildUser user, List<SocketRole> grantedRoles, OsuUserDetails osuUserDetails)
        {
            WithTitle($"Granted roles on {user.Guild.Name}:")
            .WithDescription(string.Join('\n', grantedRoles.Select(role => role.Name).OrderByDescending(role => role)))
            .WithThumbnailUrl(osuUserDetails.Avatar.ToString());
        }
    }
}