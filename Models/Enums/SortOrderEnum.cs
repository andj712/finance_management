using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace finance_management.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SortOrderEnum
    {
        [EnumMember(Value = "asc")]
        Asc,

        [EnumMember(Value = "desc")]
        Desc
    }
}
