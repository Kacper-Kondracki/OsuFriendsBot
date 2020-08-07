using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using OsuFriendsApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsuFriendsBot.Services
{
    public class OsuRoleFindingService
    {
        private readonly ILogger _logger;

        public OsuRoleFindingService(ILogger<OsuRoleFindingService> logger)
        {
            _logger = logger;
        }

        public static string DigitRole(int digit, Gamemode gamemode)
        {
            return gamemode switch
            {
                Gamemode.Std => $"[STD] {digit} DIGIT",
                Gamemode.Taiko => $"[TAIKO] {digit} DIGIT",
                Gamemode.Ctb => $"[CTB] {digit} DIGIT",
                Gamemode.Mania => $"[MANIA] {digit} DIGIT",
                Gamemode.Generic => $"{digit} DIGIT",
                _ => throw new NotImplementedException(),
            };
        }

        public string PlaystyleRole(Playstyle playstyle)
        {
            return playstyle.ToString().ToUpperInvariant();
        }

        public SocketRole FindDigitRole(IReadOnlyCollection<SocketRole> roles, int digit, Gamemode gamemode)
        {
            return roles.FirstOrDefault(role => role.Name.Equals(DigitRole(digit, gamemode), StringComparison.InvariantCultureIgnoreCase));
        }

        public List<SocketRole> FindPlaystyleRoles(IReadOnlyCollection<SocketRole> roles, List<Playstyle> playstyles)
        {
            IEnumerable<string> playstylesString = playstyles.Select(playstyle => PlaystyleRole(playstyle));
            return roles.Where(role => playstylesString.Contains(role.Name, StringComparer.InvariantCultureIgnoreCase)).ToList();
        }

        public List<SocketRole> FindUserRoles(IReadOnlyCollection<SocketRole> roles, OsuUserDetails osuUserDetails)
        {
            List<SocketRole> allRoles = FindPlaystyleRoles(roles, osuUserDetails.Playstyle);
            if (osuUserDetails.Std != null)
            {
                int std = osuUserDetails.Std.ToString().Length;
                SocketRole digitRole = FindDigitRole(roles, std, Gamemode.Std);
                allRoles.Add(digitRole);

                digitRole = FindDigitRole(roles, std, Gamemode.Generic);
                allRoles.Add(digitRole);
            }
            if (osuUserDetails.Taiko != null)
            {
                int taiko = osuUserDetails.Taiko.ToString().Length;
                SocketRole digitRole = FindDigitRole(roles, taiko, Gamemode.Taiko);
                allRoles.Add(digitRole);

                digitRole = FindDigitRole(roles, taiko, Gamemode.Generic);
                allRoles.Add(digitRole);
            }
            if (osuUserDetails.Ctb != null)
            {
                int ctb = osuUserDetails.Ctb.ToString().Length;
                SocketRole digitRole = FindDigitRole(roles, ctb, Gamemode.Ctb);
                allRoles.Add(digitRole);

                digitRole = FindDigitRole(roles, ctb, Gamemode.Generic);
                allRoles.Add(digitRole);
            }
            if (osuUserDetails.Mania != null)
            {
                int mania = osuUserDetails.Mania.ToString().Length;
                SocketRole digitRole = FindDigitRole(roles, mania, Gamemode.Mania);
                allRoles.Add(digitRole);

                digitRole = FindDigitRole(roles, mania, Gamemode.Generic);
                allRoles.Add(digitRole);
            }
            return allRoles.Distinct().Where(role => role != null).ToList();
        }

        public List<string> AllRoles()
        {
            List<string> allRoles = new List<string>();
            foreach (Playstyle playstyle in Enum.GetValues(typeof(Playstyle)))
            {
                allRoles.Add(PlaystyleRole(playstyle));
            }
            foreach (Gamemode gamemode in Enum.GetValues(typeof(Gamemode)))
            {
                for (int i = 1; i <= 7; i++)
                {
                    allRoles.Add(DigitRole(i, gamemode));
                }
            }
            return allRoles;
        }

        public List<SocketRole> FindAllRoles(IReadOnlyCollection<SocketRole> roles)
        {
            List<SocketRole> allRoles = FindPlaystyleRoles(roles, new List<Playstyle>() { Playstyle.Keyboard, Playstyle.Mouse, Playstyle.Tablet, Playstyle.Touchscreen });
            foreach (Gamemode gamemode in Enum.GetValues(typeof(Gamemode)))
            {
                for (int i = 1; i <= 7; i++)
                {
                    SocketRole digitRole = FindDigitRole(roles, i, gamemode);
                    if (digitRole != null)
                    {
                        allRoles.Add(digitRole);
                    }
                }
            }
            return allRoles;
        }
    }

    public enum Gamemode
    {
        Std,
        Taiko,
        Ctb,
        Mania,
        Generic
    }
}