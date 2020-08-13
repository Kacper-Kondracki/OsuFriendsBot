using LiteDB;
using System;

namespace OsuFriendsDb.Models
{
    public class UserData
    {
        [BsonId]
        public ulong UserId { get; set; }

        public Guid? OsuFriendsKey { get; set; }
        public int? Std { get; set; }
        public int? Taiko { get; set; }
        public int? Ctb { get; set; }
        public int? Mania { get; set; }
        public int Uwu { get; set; }
    }
}