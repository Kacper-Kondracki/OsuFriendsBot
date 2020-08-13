using Discord;
using Discord.WebSocket;
using OsuFriendsApi.Entities;
using OsuFriendsBot.Osu;
using OsuFriendsDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsuFriendsBot.Embeds
{
    public class GrantedRolesEmbed : EmbedBuilder
    {
        public GrantedRolesEmbed(SocketGuildUser user, List<SocketRole> grantedRoles, OsuUserDetails osuUserDetails, UserData userData)
        {
            Title = $"Granted roles on {user.Guild.Name}:";
            Description = string.Join('\n', grantedRoles.Select(role => role.Name).OrderByDescending(role => role));
            ThumbnailUrl = osuUserDetails.Avatar.ToString();
            Color = EmbedColors.Important;
            AddProgressField(userData.Std, osuUserDetails.Std, Gamemode.Std);
            AddProgressField(userData.Taiko, osuUserDetails.Taiko, Gamemode.Taiko);
            AddProgressField(userData.Ctb, osuUserDetails.Ctb, Gamemode.Ctb);
            AddProgressField(userData.Mania, osuUserDetails.Mania, Gamemode.Mania);

            if (osuUserDetails.Last != null)
            {
                Footer = new EmbedFooterBuilder().WithText($"Last update: {osuUserDetails.Last}");
            }
        }

        private void AddProgressField(int? oldRank, int? newRank, Gamemode gamemode)
        {
            if (oldRank != null && newRank != null && oldRank != newRank)
            {
                int difference = oldRank.Value - newRank.Value;
                AddField($"Progress in {gamemode.ToString().ToUpperInvariant()}:", $"#{oldRank} => #{newRank} ({(difference >= 0 ? "↗️" : "↘️")} {difference})");
            }
        }
    }
}