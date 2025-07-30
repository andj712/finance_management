using AutoMapper;
using CsvHelper;
using finance_management.Commands.Auto_Categorize;
using finance_management.Commands.CategorizeSingleTransaction;
using finance_management.Commands.ImportTransactions;
using finance_management.Commands.SplitTransactions;
using finance_management.DTOs.CategorizeTransaction;
using finance_management.DTOs.GetTransactions;
using finance_management.DTOs.ImportTransaction;
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

        public TransactionController(IMediator mediator)
        {
            
            _mediator = mediator;
        }

        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 440)]
        [HttpGet]
        public async Task<IActionResult> GetAllTransactions([FromQuery] GetTransactionsQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors });
            }
            catch (BusinessException ex)
            {
                return StatusCode(440, ex.Error);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred" });
            }

 
        }
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 440)]
        [HttpPost("import")]
        public async Task<IActionResult> ImportTransactions([FromForm] ImportTransactionRequest request)
        {
            try
            {
                var file = request.File;

                if (file == null || file.Length == 0)
                {
                    throw new ValidationException(new List<ValidationError>
                    {
                        new ValidationError
                        {
                            Tag = "file",
                            Error = "required",
                            Message = "CSV file is required"
                        }
                    });
                        }

                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                            throw new ValidationException(new List<ValidationError>
                    {
                        new ValidationError
                        {
                            Tag = "file",
                            Error = "invalid-format",
                            Message = "File must be a CSV file"
                        }
                    });
                }

                var command = new ImportTransactionsCommand { CsvFile = file };
                await _mediator.Send(command);
                return Ok();
            }
            catch (ValidationException vex)
            {
                return BadRequest(new ValidationResponse { Errors = vex.Errors });
            }
            catch (BusinessException bex)
            {
                return StatusCode(440, new { error = bex.Error });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred during import" });
            }
        }
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 440)]
        [HttpPost("{id}/categorize")]
        public async Task<IActionResult> CategorizeTransaction([FromRoute] string id, [FromBody] CategorizeTransactionRequest request)
        {
            try{ 
               
                if (request == null || string.IsNullOrWhiteSpace(request.CatCode))
                {
                    throw new ValidationException(new List<ValidationError>
                        {
                            new ValidationError{
                                Tag = "catcode",
                                Error = ErrorEnum.Required.ToString(),
                                Message = "Category code is required"
                            }

                   });
                }
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ValidationException(new List<ValidationError>
                        {
                            new ValidationError{
                                Tag = "id",
                                Error = ErrorEnum.Required.ToString(),
                                Message = "Id is required"
                            }

                   });
                }
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

                return Ok();
        }
             catch (ValidationException vex)
            {
                return BadRequest(new ValidationResponse { Errors = vex.Errors });
            }
            catch (BusinessException bex)
            {
                return StatusCode(440, new { error = bex.Error });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred during import" });
            }
        }


        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 440)]
        [HttpPost("{id}/split")]
        public async Task<IActionResult> Split([FromRoute] string id, [FromBody] SplitTransactionCommand command)
        {

            try
            {
                
                command.TransactionId = id;
                
                await _mediator.Send(command);
                return Ok(new { message = "Transaction split successfully" });
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

        [HttpPost("auto-categorize")]
        public async Task<IActionResult> AutoCategorize()
        {
            var categorizedCount = await _mediator.Send(new AutoCategorizeTransactionCommand());
            return Ok(new { categorized = categorizedCount });
        }
        

    }

}
