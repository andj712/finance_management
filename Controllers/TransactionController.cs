using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using finance_management.Commands;
using finance_management.Database;
using finance_management.DTOs;
using finance_management.Mapping;
using finance_management.Models;
using finance_management.Models.Validation;
using finance_management.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
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

            var mappedTransactions = transactionCommands
                .Select(cmd =>
                {
                    try { return _mapper.Map<Transaction>(cmd); }
                    catch { return null; }
                })
                .Where(x => x != null)
                .ToList();

            var allCsvIds = mappedTransactions.Select(t => t.Id).ToHashSet();

           
            var existingIds = await _db.Transactions
                .Where(t => allCsvIds.Contains(t.Id))
                .Select(t => t.Id)
                .ToListAsync();

            var existingIdSet = existingIds.ToHashSet();

            var newTransactions = mappedTransactions
                .Where(t => !existingIdSet.Contains(t.Id))
                .ToList();

            var skipped = mappedTransactions.Count - newTransactions.Count;

            if (newTransactions.Any())
            {
                await _db.Transactions.AddRangeAsync(newTransactions);
                await _db.SaveChangesAsync();
            }

            return Ok(new
            {
                imported = newTransactions.Count,
                skipped
            });
        }
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 440)]
        [HttpPost("{id}/split")]
        public async Task<IActionResult> Split(String id, [FromBody] SplitTransactionRequest request)
        {
            var original = await _db.Transactions.FindAsync(id);
            if (original == null)
                return NotFound();

            // Validacija modela
            var validator = new SplitTransactionRequestValidator();
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new ValidationError
                {
                    Tag = e.PropertyName,
                    Error = e.ErrorCode ?? "invalid",
                    Message = e.ErrorMessage
                }).ToList();

                return BadRequest(new { errors });
            }

            // split mora biti manji ili jednak originalu
            if (request.SplitAmount > original.Amount)
            {
                return StatusCode(440, new
                {
                    problem = "split-amount-over-transaction-amount",
                    message = "SplitAmount je veći od Amount originalne transakcije",
                    details = $"Originalnа Amount: {original.Amount}, SplitAmount: {request.SplitAmount}"
                });
            }

            // nova transakcija
            var remainder = original.Amount - request.SplitAmount;

            var newTransaction = new Transaction
            {
                Id = original.Id,
                BeneficiaryName = original.BeneficiaryName,
                Date = original.Date,
                Direction = original.Direction,
                Amount = request.SplitAmount,
                Description = request.NewDescription ?? original.Description,
                Currency = original.Currency,
                MccCode = original.MccCode,
                Kind = original.Kind
            };

            // u originalnoj ostaje samo ostatak
            original.Amount = remainder;

            await _db.Transactions.AddAsync(newTransaction);
            await _db.SaveChangesAsync();

            return Ok(new { original, newTransaction });
        }


    }

}
