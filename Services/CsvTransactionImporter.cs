using CsvHelper;
using CsvHelper.Configuration;
using finance_management.Commands;
using finance_management.Mapping;
using finance_management.Models;
using System.Globalization;

namespace finance_management.Services
{
    public class CsvTransactionImporter
    {
        public List<TransactionCommand> ImportTransactions(Stream csvStream)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                BadDataFound = null,
                MissingFieldFound = null,
                IgnoreBlankLines = true
            };

            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<TransactionCsvMap>();

            var transactions = new List<TransactionCommand>();

            while (csv.Read())
            {
                try
                {
                    var record = csv.GetRecord<TransactionCommand>();
                    transactions.Add(record);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Preskačem red zbog greške: {ex.Message}");
                }
            }

            return transactions;
        }

    }
}
