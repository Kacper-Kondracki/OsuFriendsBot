using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace OsuFriendsApi.Entities
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class OsuUserDetails
    {
        public OsuUserDetails(long id, string username, Uri avatar, OsuLevel level, List<Playstyle> playstyle)
        {
            Id = id;
            Username = username;
            Avatar = avatar;
            Level = level;
            Playstyle = playstyle;
        }

        public long Id { get; }
        public string Username { get; }
        public Uri Avatar { get; }
        public OsuLevel Level { get; }
        public List<Playstyle> Playstyle { get; }
    }
}