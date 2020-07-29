using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsuFriendBot.Models
{
    public class GuildSettings
    {
        [BsonId]
        public ulong GuildId { get; set; }
        public string Prefix { get; set; }
    }
}
