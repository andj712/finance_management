using CsvHelper.Configuration;
using finance_management.Models;
using finance_management.Models.Enums;
using System.ComponentModel;
using System.Globalization;
using DecimalConverter = CsvHelper.TypeConversion.DecimalConverter;
using finance_management.Models.Enums;
using finance_management.Commands;

namespace finance_management.Mapping
{
    public class TransactionCsvMap : ClassMap<TransactionCommand>
    {
        public TransactionCsvMap()
        {
            Map(m => m.Id)
            .Name("id")
            .TypeConverter<CsvHelper.TypeConversion.GuidConverter>();
            Map(m => m.BeneficiaryName)
            .Name("beneficiary-name", "BeneficiaryName"); Map(m => m.Date)
                .Name("date")
                .TypeConverterOption.Format("yyyy-MM-dd");
            Map(m => m.Direction)
                .Name("direction")
                .TypeConverter<GenericEnumConverter<DirectionEnum>>();
            Map(m => m.Amount)
                .Name("amount")
                .TypeConverter<DecimalConverter>();
            Map(m => m.Currency)
                .Name("currency");
            Map(m => m.Kind)
                .Name("kind")
                .TypeConverter<GenericEnumConverter<TransactionKindEnum>>();
            Map(m => m.MccCode)
                .Name("mcc","MccCode")
                .TypeConverter<GenericEnumConverter<MccCodeEnum>>();
        }
    }
}
