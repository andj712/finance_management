using AutoMapper;
using finance_management.Interfaces;
using finance_management.Models.Enums;
using finance_management.Validations.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace finance_management.Controllers
{
    [ApiController]
    [Route("spending-analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly ILogger<AnalyticsController> _logger;
        private readonly ICategoryService _categoriesService;
    
        public AnalyticsController(
            ILogger<AnalyticsController> logger,
            ICategoryService categoriesService)
        {
            _logger = logger;
            _categoriesService = categoriesService;
        }

        [HttpGet]
        public IActionResult ViewSpendingAnalyticsByCategory([FromQuery] string catCode,
           [FromQuery(Name = "start-date")] DateTime? startDate,
           [FromQuery(Name = "end-date")] DateTime? endDate, [FromQuery] DirectionEnum? direction)
        {
            
            var result= _categoriesService.GetSpendingAnalyticsByCategory(catCode, startDate, endDate, direction);

            
            return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
        }


    }
}
