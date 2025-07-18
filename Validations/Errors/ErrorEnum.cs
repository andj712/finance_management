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
        
    }
}
