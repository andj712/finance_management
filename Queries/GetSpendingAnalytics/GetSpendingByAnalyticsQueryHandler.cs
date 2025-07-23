using finance_management.Database;
using finance_management.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace finance_management.Queries.GetSpendingAnalytics
{
    public class GetSpendingAnalyticsQueryHandler : IRequestHandler<GetSpendingAnalyticsQuery, SpendingAnalytics>
    {
        private readonly PfmDbContext _context;

        public GetSpendingAnalyticsQueryHandler(PfmDbContext context)
        {
            _context = context;
        }

        public async Task<SpendingAnalytics> Handle(GetSpendingAnalyticsQuery request, CancellationToken cancellationToken)
        {
            var startDate = request.StartDate ?? new DateTime(2021, 1, 1);
            var endDate = (request.EndDate ?? DateTime.Today) > DateTime.Today ? DateTime.Today : request.EndDate ?? DateTime.Today;

            var transactionsQuery = _context.Transactions.AsQueryable()
                .Where(t => t.Date > startDate && t.Date < endDate && t.CatCode != null);

            if (!string.IsNullOrEmpty(request.CatCode))
            {
                transactionsQuery = transactionsQuery.Where(t => t.CatCode == request.CatCode);
            }

            if (request.Direction != null)
            {
                transactionsQuery = transactionsQuery.Where(t => t.Direction == request.Direction);
            }

            var grouped = await transactionsQuery
                .GroupBy(t => t.CatCode)
                .Select(g => new SpendingAnalyticsInCategory
                {
                    CatCode = g.Key,
                    Amount = (double)Math.Round(g.Sum(t => t.Amount), 2),
                    Count = g.Count()
                })
                .ToListAsync(cancellationToken);

            return new SpendingAnalytics
            {
                Groups = grouped
            };
        }
    }
}
