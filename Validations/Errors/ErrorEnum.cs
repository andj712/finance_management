using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace finance_management.Validations.Errors
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ErrorEnum
    {
        InvalidFormat,
        MaxLength,
        Required,
        InvalidValue,
        CheckDigitInvalid,
        Duplicate,
        Invalid
    }

}

