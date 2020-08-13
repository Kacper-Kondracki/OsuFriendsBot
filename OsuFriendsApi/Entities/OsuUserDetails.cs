using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace OsuFriendsApi.Entities
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class OsuUserDetails
    {
        public OsuUserDetails(long id, string username, Uri avatar, int? std, int? mania, int? taiko, int? ctb, List<Playstyle> playstyle, string last)
        {
            Id = id;
            Username = username;
            Avatar = avatar;
            Std = std;
            Mania = mania;
            Taiko = taiko;
            Ctb = ctb;
            Playstyle = playstyle;
            Last = last;
        }

        public long Id { get; }
        public string Username { get; }
        public Uri Avatar { get; }
        public int? Std { get; }
        public int? Taiko { get; }
        public int? Ctb { get; }
        public int? Mania { get; }
        public List<Playstyle> Playstyle { get; }
        public string Last { get; }
    }
}