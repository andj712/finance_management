using AutoMapper;
using CsvHelper;
using finance_management.Commands.CategorizeSingleTransaction;
using finance_management.Commands.ImportTransactions;
using finance_management.Commands.SplitTransactions;
using finance_management.DTOs.CategorizeTransaction;
using finance_management.DTOs.GetTransactions;
using finance_management.Models;
using finance_management.Queries.GetTransactions;
using finance_management.Validations.Errors;
using finance_management.Validations.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using ValidationException = finance_management.Validations.Exceptions.ValidationException;


namespace finance_management.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionController : ControllerBase
    {
   
        private readonly IMediator _mediator;
        private IMapper _mapper;

        public TransactionController(IMediator mediator,IMapper mapper)
        {
            
            _mediator = mediator;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllTransactions([FromQuery] GetTransactionsQuery query)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(query);
            if (!Validator.TryValidateObject(query, context, validationResults, true))
            {
                var errors = validationResults.Select(e => new ValidationError
                {
                    Tag = e.MemberNames.FirstOrDefault() ?? "unknown",
                    Error = ErrorEnum.InvalidValue.ToString(),
                    Message = e.ErrorMessage ?? "Validation failed"
                }).ToList();

                throw new ValidationException(errors);
            }

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportTransactions([FromForm] ImportTransactionsCommand command)
        {
            if (command?.CsvFile == null || command.CsvFile.Length == 0)
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

        [HttpPost("{id}/categorize")]
        public async Task<IActionResult> CategorizeTransaction([FromRoute] string id, [FromBody] CategorizeTransactionRequest request)
        {
            var command = new CategorizeTransactionCommand
            {
                TransactionId = id,
                CatCode = request.CatCode
            };

            var result = await _mediator.Send(command);

            if (result.ValidationErrors.Any())
            {
                return BadRequest(new ValidationResponse
                {
                    Errors = result.ValidationErrors
                });
            }

            if (result.BusinessError != null)
            {
                return StatusCode(440, result.BusinessError);
            }

            return Ok(new
            {
                message = "Transaction categorized successfully"
            });
        }


        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 440)]
        [HttpPost("{id}/split")]
        public async Task<IActionResult> Split([FromRoute] string id, [FromBody] List<SingleCategorySplit> splits )
        {
            try
            {
                var command = new SplitTransactionCommand
                {
                    TransactionId = id,
                    Splits = splits,
                };
                await _mediator.Send(command);
                return Ok();
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ValidationResponse { Errors = ex.Errors });
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Error);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Database error occurred" });
            }
            catch (Exception ex)
            {
                // Log the error  
                return StatusCode(500, new { message = "An unexpected error occurred" });
            }
        }

       


    }

}
