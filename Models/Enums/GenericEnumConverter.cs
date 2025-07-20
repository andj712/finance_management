using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Runtime.Serialization;

namespace finance_management.Models.Enums
{
    public class GenericEnumConverter<T> : DefaultTypeConverter where T : struct, Enum
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                // Ako je nullable enum, vrati null
                var underlyingType = Nullable.GetUnderlyingType(typeof(T));
                if (underlyingType != null)
                    return null;

                // Ako nije nullable, vrati default vrednost
                return default(T);
            }

            foreach (var field in typeof(T).GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(EnumMemberAttribute)) as EnumMemberAttribute;

                if (attribute != null && attribute.Value == text)
                {
                    return Enum.Parse(typeof(T), field.Name);
                }
            }

            if (Enum.TryParse<T>(text, true, out var result))
                return result;

            throw new ArgumentException($"Cannot convert '{text}' to {typeof(T).Name}");
        }
    }
}
