namespace OsuFriendsBot.Osu
{
    using Discord.WebSocket;
    using OsuFriendsApi.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    namespace OsuFriendsBot.Services
    {
        public static class OsuRoles
        {
            /// <summary>
            /// Translates digit and gamemode into useful string.
            /// </summary>
            /// <param name="digit">Rank digit.</param>
            /// <param name="gamemode">Gamemode.</param>
            /// <returns>String containing info about digit on gamemode.</returns>
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

            /// <summary>
            /// Gets playstyle role string.
            /// </summary>
            /// <param name="playstyle">Playstyle.</param>
            /// <returns>Playstyle string.</returns>
            public static string PlaystyleRole(Playstyle playstyle)
            {
                return playstyle.ToString().ToUpperInvariant();
            }

            /// <summary>
            /// Finds SocketRole that matches the digit role string.
            /// </summary>
            /// <param name="roles">Guild roles.</param>
            /// <param name="digit">Rank digit.</param>
            /// <param name="gamemode">Gamemode.</param>
            /// <returns>Role that matches the digit role string.</returns>
            public static SocketRole FindDigitRole(IReadOnlyCollection<SocketRole> roles, int digit, Gamemode gamemode)
            {
                return roles.FirstOrDefault(role => role.Name.Equals(DigitRole(digit, gamemode), StringComparison.InvariantCultureIgnoreCase));
            }

            /// <summary>
            /// Finds SocketRoles that matches the playstyles roles strings.
            /// </summary>
            /// <param name="roles">Guild roles.</param>
            /// <param name="playstyles">Playstyles.</param>
            /// <returns>Roles that matches the playstyles roles strings.</returns>
            public static List<SocketRole> FindPlaystyleRoles(IReadOnlyCollection<SocketRole> roles, List<Playstyle> playstyles)
            {
                IEnumerable<string> playstylesString = playstyles.Select(playstyle => PlaystyleRole(playstyle));
                return roles.Where(role => playstylesString.Contains(role.Name, StringComparer.InvariantCultureIgnoreCase)).ToList();
            }

            /// <summary>
            /// Finds SocketRoles that the user should have.
            /// </summary>
            /// <param name="roles">Guild roles.</param>
            /// <param name="osuUserDetails">User details.</param>
            /// <returns>Roles that the user should have.</returns>
            public static List<SocketRole> FindUserRoles(IReadOnlyCollection<SocketRole> roles, OsuUserDetails osuUserDetails)
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

            /// <summary>
            /// Gets all available roles names
            /// </summary>
            /// <returns>Roles names</returns>
            public static List<string> AllRoles()
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

            /// <summary>
            /// Finds all configured roles on the guild.
            /// </summary>
            /// <param name="roles">Guild roles.</param>
            /// <returns>Guild roles.</returns>
            public static List<SocketRole> FindAllRoles(IReadOnlyCollection<SocketRole> roles)
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
    }
}