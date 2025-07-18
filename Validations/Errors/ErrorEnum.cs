using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace finance_management.Validations.Errors
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ErrorEnum
    {
        [Description("CSV file is empty")]
        [EnumMember(Value = "empty-file")]
        EmptyFile,

        [Description("Mandatory field or parameter was not supplied")]
        [EnumMember(Value = "required")]
        Required,

        [Description("Value supplied does not have expected format")]
        [EnumMember(Value = "invalid-format")]
        InvalidFormat

    }
}
