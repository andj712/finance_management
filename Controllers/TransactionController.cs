using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using finance_management.Commands;
using finance_management.Database;
using finance_management.DTOs;
using finance_management.Mapping;
using finance_management.Models;
using finance_management.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Formats.Asn1;
using System.Globalization;

namespace finance_management.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly PfmDbContext _db;
        private readonly IMapper _mapper;

        private readonly CsvTransactionImporter _csvImporter;

        public TransactionController(PfmDbContext db, CsvTransactionImporter csvImporter,IMapper mapper)
        {
            _db = db;
            _csvImporter = csvImporter;
            _mapper= mapper;
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import([FromForm] ImportTransactionsRequest request)
        {
            var file = request.File;
            if (file == null || file.Length == 0)
                return BadRequest("CSV fajl nije prosleđen.");

            List<TransactionCommand> transactionCommands;
            try
            {
                using var stream = file.OpenReadStream();
                transactionCommands = _csvImporter.ImportTransactions(stream);
            }
            catch (CsvHelperException ex)
            {
                return BadRequest($"Pogrešan CSV format: {ex.Message}");
            }
            var transactions=_mapper.Map<List<Transaction>>(transactionCommands);
            await _db.Transactions.AddRangeAsync(transactions);
            await _db.SaveChangesAsync();

            return Ok(new { imported = transactionCommands.Count });
        }

    }
}
