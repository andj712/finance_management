using finance_management.Commands;
using finance_management.Commands.ImportCategories;
using finance_management.Validations.Errors;
using finance_management.Validations.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace finance_management.Controllers
{
    [ApiController]
    [Route("categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportCategories(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
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

                var command = new ImportCategoriesCommand { File = file };
                var result = await _mediator.Send(command);

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
                return StatusCode(500, new { message = "An error occurred while processing the request" });
            }
        }

        
    }
}