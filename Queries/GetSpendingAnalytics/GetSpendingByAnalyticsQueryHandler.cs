using finance_management.Database;
using finance_management.Interfaces;
using finance_management.Models;
using finance_management.Validations.Errors;
using finance_management.Validations.Exceptions;
using finance_management.Validations.Log;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace finance_management.Queries.GetSpendingAnalytics
{
    public class GetSpendingAnalyticsQueryHandler
    : IRequestHandler<GetSpendingAnalyticsQuery, SpendingAnalytics>
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly SpendingAnalyticsErrorLoggingService _errorLogger;

        public GetSpendingAnalyticsQueryHandler(ICategoryRepository categoryRepo, SpendingAnalyticsErrorLoggingService errorLogger)
        {
            _categoryRepo = categoryRepo;
            _errorLogger = errorLogger;
        }

        public async Task<SpendingAnalytics> Handle(GetSpendingAnalyticsQuery request, CancellationToken cancellationToken)
        {
            // Validate the request
            var validationErrors = ValidateRequest(request);
            if (validationErrors.Any())
            {
                throw new ValidationException(validationErrors);
            }

            // Check if category exists (only if catCode is provided and not null/empty)
            if (!string.IsNullOrWhiteSpace(request.CatCode))
            {
                var exists = await _categoryRepo.ExistsAsync(request.CatCode);
                if (!exists)
                {
                    var businessError = new BusinessError
                    {
                        Problem = "InvalidCategoryCode",
                        Message = $"Category '{request.CatCode}' does not exist.",
                        Details = "The specified category code was not found in the database."
                    };
                  //  await _errorLogger.LogSpendingBusinessErrorAsync(businessError);
                    throw new BusinessException(businessError);
                }
            }

            // Adjust end date if it's in the future (business rule)
            if (request.EndDate.HasValue && request.EndDate > DateTime.Today)
                request.EndDate = DateTime.Today;

            var analytics = await _categoryRepo.GetSpendingAnalyticsAsync(
                request.CatCode,
                request.StartDate,
                request.EndDate,
                request.Direction);

            return analytics;
        }

        private List<ValidationError> ValidateRequest(GetSpendingAnalyticsQuery request)
        {
            var errors = new List<ValidationError>();

            if (!string.IsNullOrWhiteSpace(request.CatCode))
            {
                if (request.CatCode.Length > 10)
                {
                    errors.Add(new ValidationError
                    {
                        Tag = "cat-code",
                        Error = "max-length",
                        Message = "Category code cannot exceed 10 characters"
                    });
                }

              
            }

            if (request.StartDate.HasValue)
            {
                if (request.StartDate.Value > DateTime.Today)
                {
                    errors.Add(new ValidationError
                    {
                        Tag = "start-date",
                        Error = "invalid-value",
                        Message = "Start date cannot be in the future"
                    });
                }
            }

            if (request.EndDate.HasValue)
            {
                if (request.EndDate.Value < new DateTime(2010, 1, 1))
                {
                    errors.Add(new ValidationError
                    {
                        Tag = "end-date",
                        Error = "invalid-value",
                        Message = "End date is too far in the past"
                    });
                }
            }

            // start date mora biti pre ili jednak end date
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                if (request.StartDate.Value > request.EndDate.Value)
                {
                    errors.Add(new ValidationError
                    {
                        Tag = "date-range",
                        Error = "invalid-value",
                        Message = "Start date cannot be after end date"
                    });
                }

                //najveci range godinu dana
                var dateRange = request.EndDate.Value - request.StartDate.Value;
                if (dateRange.TotalDays > 365)
                {
                    errors.Add(new ValidationError
                    {
                        Tag = "date-range",
                        Error = "invalid-value",
                        Message = "Date range cannot exceed 365 days"
                    });
                }
            }

            return errors;
        }

    


    }
}
