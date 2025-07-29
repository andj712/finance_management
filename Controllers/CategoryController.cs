using finance_management.Commands;
using finance_management.Commands.ImportCategories;
using finance_management.DTOs.ImportCategory;
using finance_management.Interfaces;
using finance_management.Queries.GetCategories;
using finance_management.Services;
using finance_management.Validations.Errors;
using finance_management.Validations.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;


namespace finance_management.Controllers
{
    [ApiController]
    [Route("categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRedisService _redis;

        public CategoriesController(IMediator mediator,IRedisService redis)
        {
            _mediator = mediator;
            _redis = redis;

        }

        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportCategories([FromForm] ImportCategoriesCommand command)
        {
            try
            {
                if ( command?.File == null || command.File.Length == 0)
                {
                    return BadRequest(new ValidationResponse
                    {
                        Errors = new List<ValidationError>
                        {
                            new ValidationError
                            {
                                Tag = "file",
                                Error = "required",
                                Message = "File is required"
                            }
                        }
                    });
                }

                var result = await _mediator.Send(command);

                var deleted = await _redis.RemoveKeysByPatternAsync("categories:parent:*");
                Console.WriteLine($"Removed {deleted} category parent keys from Redis");

                return Ok(new { categories = result });

            }
            catch (ValidationException ex)
            {
                return BadRequest(new ValidationResponse { Errors = ex.Errors });
            }
            catch (BusinessException ex)
            {
                return StatusCode(440, ex.Error);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "An error occurred while processing the request" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery(Name = "parent-id")] string? parentId)
        {
            try
            {
                var redisKey = string.IsNullOrWhiteSpace(parentId)
                    ? "categories:parent:root"
                    : $"categories:parent:{parentId}";

                var cached = await _redis.GetObjectAsync<List<CategoryDto>>(redisKey);
                if (cached is not null)
                {
                    Console.WriteLine("From Redis");
                    return Ok(new { categories = cached });
                }

                var query = new GetCategoriesQuery { ParentId = parentId };
                var result = await _mediator.Send(query);

                await _redis.SetObjectAsync(redisKey, result, TimeSpan.FromMinutes(30));

                return Ok(new { categories = result });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ValidationResponse { Errors = ex.Errors });
            }
            catch (BusinessException ex)
            {
                return StatusCode(440, ex.Error);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "An error occurred while processing the request" });
            }
        }


    }
}