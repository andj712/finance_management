using CsvHelper;
using CsvHelper.Configuration;
using finance_management.DTOs.ImportTransaction;
using System.Globalization;

namespace finance_management.Services
{
    public class CsvProcessingService
    {

        public async Task<(List<TransactionCsvDto> records, string[] headers)> ParseCsvAsync(Stream csvStream)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                PrepareHeaderForMatch = args => args.Header.ToLower().Replace("-", "")
            };

            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, config);

            var records = new List<TransactionCsvDto>();
            var headers = Array.Empty<string>();

            await csv.ReadAsync();
            csv.ReadHeader();
            headers = csv.HeaderRecord ?? Array.Empty<string>();

            while (await csv.ReadAsync())
            {
                try
                {
                    var record = new TransactionCsvDto
                    {
                        Id = csv.GetField("id") ?? string.Empty,
                        BeneficiaryName = csv.GetField("beneficiary-name"),
                        Date = csv.GetField("date") ?? string.Empty,
                        Direction = csv.GetField("direction") ?? string.Empty,
                        Amount = csv.GetField("amount") ?? string.Empty,
                        Description = csv.GetField("description"),
                        Currency = csv.GetField("currency") ?? string.Empty,
                        Mcc = csv.GetField("mcc"),
                        Kind = csv.GetField("kind") ?? string.Empty
                    };
                    records.Add(record);
                }
                catch
                {
                    continue;
                }
            }

            return (records, headers);
        }
    }
}
