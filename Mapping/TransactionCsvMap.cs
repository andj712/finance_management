using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using finance_management.Commands;
using finance_management.DTOs.ImportTransaction;
using finance_management.Models;
using finance_management.Models.Enums;
using finance_management.Models.Enums;
using System.ComponentModel;
using System.Globalization;
using DecimalConverter = CsvHelper.TypeConversion.DecimalConverter;

namespace finance_management.Mapping
{
    public sealed class TransactionCsvMap : ClassMap<TransactionCsvDto>
    {
        public TransactionCsvMap()
        {
            Map(m => m.Id).Name("id");
            Map(m => m.BeneficiaryName).Name("beneficiary-name");
            Map(m => m.Date).Name("date");
            Map(m => m.Direction).Name("direction");
            Map(m => m.Amount).Name("amount");
            Map(m => m.Description).Name("description");
            Map(m => m.Currency).Name("currency");
            Map(m => m.Mcc).Name("mcc");
            Map(m => m.Kind).Name("kind");
        }
    }
}
