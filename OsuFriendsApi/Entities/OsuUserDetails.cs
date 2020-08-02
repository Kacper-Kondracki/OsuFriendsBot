using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace OsuFriendsApi.Entities
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class OsuUserDetails
    {
        public OsuUserDetails(long id, string username, Uri avatar, long? std, long? mania, long? taiko, long? ctb, List<Playstyle> playstyle)
        {
            Id = id;
            Username = username;
            Avatar = avatar;
            Std = std;
            Mania = mania;
            Taiko = taiko;
            Ctb = ctb;
            Playstyle = playstyle;
        }

        public long Id { get; }
        public string Username { get; }
        public Uri Avatar { get; }
        public long? Std { get; }
        public long? Taiko { get; }
        public long? Ctb { get; }
        public long? Mania { get; }
        public List<Playstyle> Playstyle { get; }
    }
}