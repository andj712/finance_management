using AutoMapper;
using finance_management.Interfaces;
using finance_management.Models.Enums;
using finance_management.Queries.GetSpendingAnalytics;
using finance_management.Validations.Errors;
using finance_management.Validations.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;

namespace finance_management.Controllers
{
    [ApiController]
    [Route("spending-analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly ILogger<AnalyticsController> _logger;
        private readonly ICategoryService _categoriesService;
        private readonly IMediator _mediator;
        public AnalyticsController(
            ILogger<AnalyticsController> logger,
            ICategoryService categoriesService,
            IMediator mediator)
        {
            _logger = logger;
            _categoriesService = categoriesService;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> ViewSpendingAnalyticsByCategory([FromQuery(Name = "catcode")] string catCode,
           [FromQuery(Name = "start-date")] DateTime? startDate,
           [FromQuery(Name = "end-date")] DateTime? endDate, [FromQuery] DirectionEnum? direction)
        {
            try
            {
                var query = new GetSpendingAnalyticsQuery
                {
                    CatCode = catCode,
                    StartDate = startDate,
                    EndDate = endDate,
                    Direction = direction
                };

                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (ValidationException ex)
            {
          
                var response = new ValidationResponse
                {
                    Errors = ex.Errors
                };
                return BadRequest(response);
            }
            catch (BusinessException ex)
            {
                return StatusCode(440, ex.Error);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An internal server error occurred" });
            }
        }


    }
}
