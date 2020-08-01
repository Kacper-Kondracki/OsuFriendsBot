using LiteDB;

namespace OsuFriendBot.Models
{
    public class GuildSettings
    {
        [BsonId]
        public ulong GuildId { get; set; }

        public string Prefix { get; set; }
    }
}