using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace OsuFriendsApi.Entities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Playstyle
    {
        [EnumMember(Value = "touch")]
        Touchscreen,

        Tablet,
        Mouse,
        Keyboard,
    }
}