using CsvHelper.Configuration;
using finance_management.Models;
using System.ComponentModel;
using System.Globalization;
using DecimalConverter = CsvHelper.TypeConversion.DecimalConverter;

namespace finance_management.Mapping
{
    public class TransactionCsvMap : ClassMap<Transaction>
    {
        public TransactionCsvMap()
        {
            Map(m => m.Id).Name("id");
            Map(m => m.BeneficiaryName).Name("beneficiary-name");
            Map(m => m.Date)
            .Name("date")
            .Convert(row =>
            {
                var date = row.Row.GetField("date");
                var parsed = DateTime.Parse(date, CultureInfo.InvariantCulture);
                return DateTime.SpecifyKind(parsed, DateTimeKind.Utc);
            });
            Map(m => m.Direction).Name("direction");
            Map(m => m.Amount)
                .Name("amount")
                .TypeConverter<DecimalConverter>()
                .TypeConverterOption.CultureInfo(CultureInfo.InvariantCulture);
            Map(m => m.Description).Name("description");
            Map(m => m.Currency).Name("currency");
            Map(m => m.Mcc).Name("mcc");
            Map(m => m.Kind).Name("kind");
        }
    }
}
