using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OsuFriendsApi.Entities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Playstyle
    {
        Keyboard,
        Tablet,
        Touchscreen,
        Mouse
    }
}