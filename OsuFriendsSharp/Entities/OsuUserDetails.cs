using System.Collections.Generic;

namespace OsuFriendsSharp.Entities
{
    public class OsuUserDetails
    {
        public string Username { get; set; }
        public int StdRank { get; set; }
        public int CtbRank { get; set; }
        public int TaikoRank { get; set; }
        public int ManiaRank { get; set; }
        public List<Playstyle> Playstyle { get; set; }
    }
}