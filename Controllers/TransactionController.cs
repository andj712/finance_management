using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using finance_management.Commands.CategorizeSingleTransaction;
using finance_management.Commands.ImportTransactions;
using finance_management.Commands.SplitTransactions;
using finance_management.Database;
using finance_management.DTOs;
using finance_management.DTOs.CategorizeTransaction;
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
using System.Net.WebSockets;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;



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
        public async Task<IActionResult> Split([FromRoute] string id, [FromBody] SplitTransactionCommand command)
        {
            command.TransactionId = id;
            var result = await _mediator.Send(command);
            return Ok();
        }

       


    }

}
