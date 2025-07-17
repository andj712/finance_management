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
                .TypeConverterOption.Format("M/d/yyyy")
                .TypeConverterOption.CultureInfo(CultureInfo.InvariantCulture);
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
