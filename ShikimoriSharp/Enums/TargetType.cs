using System.Runtime.Serialization;

namespace ShikimoriSharp.Enums
{
    public enum TargetType
    {
        [EnumMember(Value = "Anime")]
        Anime,
        [EnumMember(Value = "Manga")]
        Manga
    }
}