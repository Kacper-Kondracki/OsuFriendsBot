using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OsuFriendsApi.Entities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Status
    {
        Completed,
        Pending,
        Invalid
    }
}