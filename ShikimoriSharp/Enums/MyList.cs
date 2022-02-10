using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ShikimoriSharp.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MyList
    {
        [EnumMember(Value = "planned")]
        Planned,
        [EnumMember(Value = "watching")]
        Watching,
        [EnumMember(Value = "rewatching")]
        ReWatching,
        [EnumMember(Value = "completed")]
        Completed,
        [EnumMember(Value = "on_hold")]
        OnHold,
        [EnumMember(Value = "dropped")]
        Dropped
    }
}