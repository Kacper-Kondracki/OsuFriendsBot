using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OsuFriendsApi.Entities
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class OsuLevelInfo
    {
        public OsuLevelInfo(long current, long progress)
        {
            Current = current;
            Progress = progress;
        }

        public long Current { get; }
        public long Progress { get; }
    }
}