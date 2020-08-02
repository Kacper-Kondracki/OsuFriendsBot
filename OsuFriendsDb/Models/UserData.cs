using LiteDB;
using System;

namespace OsuFriendsDb.Models
{
    public class UserData
    {
        [BsonId]
        public ulong UserId { get; set; }

        public Guid OsuFriendsKey { get; set; }
    }
}