using CsvHelper;
using CsvHelper.Configuration;
using finance_management.Database;
using finance_management.DTOs;
using finance_management.Mapping;
using finance_management.Models;
using Microsoft.AspNetCore.Mvc;
using System.Formats.Asn1;
using System.Globalization;

namespace finance_management.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly PfmDbContext _db;

        public TransactionController(PfmDbContext db)
        {
            _db = db;
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import([FromForm] ImportTransactionsRequest request)
        {
            var file = request.File;
            if (file == null || file.Length == 0) return BadRequest("CSV fajl nije prosleđen.");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ","
            };

            List<Transaction> transactions;
            using (var stream = file.OpenReadStream())
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, config))
            {
                // **OVDE** registrujemo mapu
                csv.Context.RegisterClassMap<TransactionCsvMap>();

                try
                {
                    transactions = csv.GetRecords<Transaction>().ToList();
                }
                catch (CsvHelperException ex)
                {
                    return BadRequest($"Pogrešan CSV format: {ex.Message}");
                }
            }

            await _db.Transactions.AddRangeAsync(transactions);
            await _db.SaveChangesAsync();

            return Ok(new { imported = transactions.Count });
        }
    }
}
