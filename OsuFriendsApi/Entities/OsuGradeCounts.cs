using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OsuFriendsApi.Entities
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class OsuGradeCounts
    {
        public OsuGradeCounts(long ss, long ssh, long s, long sh, long a)
        {
            SS = ss;
            SSH = ssh;
            S = s;
            SH = sh;
            A = a;
        }

        public long SS { get; }
        public long SSH { get; }
        public long S { get; }
        public long SH { get; }
        public long A { get; }
    }
}