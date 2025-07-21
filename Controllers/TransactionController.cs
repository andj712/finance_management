using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using finance_management.Commands;
using finance_management.Database;
using finance_management.DTOs;
using finance_management.DTOs.ImportTransaction;
using finance_management.Interfaces;
using finance_management.Mapping;
using finance_management.Models;
using finance_management.Queries.GetTransactions;
using finance_management.Services;
using finance_management.Validations.Errors;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Formats.Asn1;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Errors = finance_management.Validations.Errors.ValidationError;


namespace finance_management.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly PfmDbContext _db;
        private readonly IMapper _mapper;

        private readonly ITransactionService _transactionService;
        private readonly IMediator _mediator;

        public TransactionController(PfmDbContext db,IMapper mapper, ITransactionService transactionService,IMediator mediator)
        {
            _db = db;
            _mapper= mapper;
            _transactionService = transactionService;
            _mediator = mediator;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllTransactions([FromQuery] GetTransactionsQueryDTO queryDto)
        {
            
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(queryDto);

            if (!Validator.TryValidateObject(queryDto, validationContext, validationResults, true))
            {
                foreach (var error in validationResults)
                {
                    foreach (var memberName in error.MemberNames)
                    {
                        ModelState.AddModelError(memberName, error.ErrorMessage);
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var query = new GetTransactionsQuery
                {
                    TransactionKind = queryDto.TransactionKind,
                    StartDate = queryDto.StartDate,
                    EndDate = queryDto.EndDate,
                    Page = queryDto.Page,
                    PageSize = queryDto.PageSize,
                    SortBy = queryDto.SortBy,
                    SortOrder = queryDto.SortOrder
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportTransactions(IFormFile file)
        {
            if (file == null)
            {
                return BadRequest(new ValidationResponse
                {
                    Errors = new List<ValidationError>
                    {
                        new ValidationError
                        {
                            Tag = "file",
                            Error = ErrorEnum.Required.ToString(),
                            Message = "CSV file is required"
                        }
                    }
                });
            }

            var command = new ImportTransactionsCommand(file);
            var result = await _mediator.Send(command);

            if (result.ValidationErrors.Any() && result.ImportedCount == 0)
            {
                return BadRequest(new ValidationResponse
                {
                    Errors = result.ValidationErrors
                });
            }

            //return Ok(new
            //{
            //    message = "Import completed",
            //    processedCount = result.ProcessedCount,
            //    importedCount = result.ImportedCount,
            //    skippedCount = result.SkippedCount,
            //    logFileName = result.LogFileName,
            //    errors = result.ValidationErrors
            //});
            //da ne bi vracao error i logfile ako je prazna lista gresaka koristila sam dictionary
            var response = new Dictionary<string, object>
            {
                ["message"] = "Import completed",
                ["processedCount"] = result.ProcessedCount,
                ["importedCount"] = result.ImportedCount,
                ["skippedCount"] = result.SkippedCount,
            };

            if (result.ValidationErrors != null && result.ValidationErrors.Any())
            {
                response["logFileName"] = result.LogFileName;
                response["errors"] = result.ValidationErrors;
            }

            return Ok(response);
        }
    


        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 440)]
        [HttpPost("{id}/split")]
        public async Task<IActionResult> Split(string id, [FromBody] SplitTransactionRequest request)
        {
            var original = await _db.Transactions.FindAsync(id);
            if (original == null)
                return NotFound();

            // Validacija modela
            var validator = new SplitTransactionRequestValidator();
            var validationResult = await validator.ValidateAsync(request);
            

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
                MccCode= original.MccCode,
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
