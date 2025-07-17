using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace finance_management.Models.Enums
{
    public class GenericEnumConverter<T> : DefaultTypeConverter where T : struct, Enum
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (Enum.TryParse<T>(text, true, out var result))
                return result;

            throw new TypeConverterException(this, memberMapData, text, row.Context,
                $"Ne mogu parsirati '{text}' u enum {typeof(T).Name}");
        }
    }
}
