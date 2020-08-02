using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OsuFriendsApi.Entities
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class OsuRank
    {
        public OsuRank(long global, long country)
        {
            Global = global;
            Country = country;
        }

        public long Global { get; }
        public long Country { get; }
    }
}