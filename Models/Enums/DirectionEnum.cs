using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace finance_management.Models.Enums
{

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DirectionEnum
    {
        [EnumMember(Value = "d")]
        Debit,

        [EnumMember(Value = "c")]
        Credit
    }

}
