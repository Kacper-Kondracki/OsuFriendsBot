using Discord;
using Discord.WebSocket;
using OsuFriendsApi.Entities;
using OsuFriendsBot.Osu;
using OsuFriendsDb.Models;
using System.Collections.Generic;
using System.Linq;

namespace OsuFriendsBot.Embeds
{
    public class GrantedRolesEmbed : EmbedBuilder
    {
        public GrantedRolesEmbed(SocketGuildUser user, List<SocketRole> grantedRoles, OsuUserDetails osuUserDetails, UserData userData)
        {
            WithTitle($"Granted roles on {user.Guild.Name}:")
            .WithDescription(string.Join('\n', grantedRoles.Select(role => role.Name).OrderByDescending(role => role)))
            .WithThumbnailUrl(osuUserDetails.Avatar.ToString())
            .WithColor(Discord.Color.Gold);

            AddProgressField(userData.Std, osuUserDetails.Std, Gamemode.Std);
            AddProgressField(userData.Taiko, osuUserDetails.Taiko, Gamemode.Taiko);
            AddProgressField(userData.Ctb, osuUserDetails.Ctb, Gamemode.Ctb);
            AddProgressField(userData.Mania, osuUserDetails.Mania, Gamemode.Mania);
        }

        private void AddProgressField(int? oldRank, int? newRank, Gamemode gamemode)
        {
            if (oldRank != null && newRank != null && oldRank != newRank)
            {
                AddField($"Progress in {gamemode.ToString().ToUpperInvariant()}:", $"#{oldRank} => #{newRank}");
            }
        }
    }
}