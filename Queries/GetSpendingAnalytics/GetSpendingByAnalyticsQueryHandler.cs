using finance_management.Database;
using finance_management.Interfaces;
using finance_management.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace finance_management.Queries.GetSpendingAnalytics
{
    public class GetSpendingAnalyticsQueryHandler
    : IRequestHandler<GetSpendingAnalyticsQuery, SpendingAnalytics>
    {
        private readonly ICategoryRepository _categoryRepo;

        public GetSpendingAnalyticsQueryHandler(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<SpendingAnalytics> Handle(GetSpendingAnalyticsQuery request, CancellationToken cancellationToken)
        {

            if (request.EndDate.HasValue && request.EndDate > DateTime.Today)
                request.EndDate = DateTime.Today;

            var analytics = await _categoryRepo
                .GetSpendingAnalyticsAsync(
                    request.CatCode,
                    request.StartDate,
                    request.EndDate,
                    request.Direction);

            return analytics;
        }
    }
}
