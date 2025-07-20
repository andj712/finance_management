using finance_management.Database;
using finance_management.Models;
using finance_management.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace finance_management.Queries.GetTransactions
{
    public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, List<Transaction>>
    {
        private readonly PfmDbContext _db;

        public GetTransactionsQueryHandler(PfmDbContext db)
        {
            _db = db;
        }

        public async Task<List<Transaction>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            var query = _db.Transactions.AsQueryable();

            // Filter by transaction kind
            if (!string.IsNullOrEmpty(request.TransactionKind) &&
            Enum.TryParse<TransactionKindEnum>(request.TransactionKind, true, out var parsedKind))
            {
                query = query.Where(t => t.Kind == parsedKind);
            }

            // Filter by date range, convert to UTC
            if (request.StartDate.HasValue)
            {
                var utcStartDate = request.StartDate.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(request.StartDate.Value, DateTimeKind.Utc)
                    : request.StartDate.Value.ToUniversalTime();
                query = query.Where(t => t.Date >= utcStartDate);
            }

            if (request.EndDate.HasValue)
            {
                var utcEndDate = request.EndDate.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(request.EndDate.Value, DateTimeKind.Utc)
                    : request.EndDate.Value.ToUniversalTime();
                query = query.Where(t => t.Date <= utcEndDate);
            }

            // Sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                switch (request.SortBy.ToLower())
                {
                    case "date":
                        query = request.SortOrder.ToLower() == "desc"
                            ? query.OrderByDescending(t => t.Date)
                            : query.OrderBy(t => t.Date);
                        break;
                    case "amount":
                        query = request.SortOrder.ToLower() == "desc"
                            ? query.OrderByDescending(t => t.Amount)
                            : query.OrderBy(t => t.Amount);
                        break;
                    case "beneficiaryname":
                        query = request.SortOrder.ToLower() == "desc"
                            ? query.OrderByDescending(t => t.BeneficiaryName)
                            : query.OrderBy(t => t.BeneficiaryName);
                        break;
                    default:
                        query = query.OrderByDescending(t => t.Date);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(t => t.Date);
            }

            // Pagination
            var skip = (request.Page - 1) * request.PageSize;
            query = query.Skip(skip).Take(request.PageSize);

            return await query.ToListAsync(cancellationToken);
        }
    }
}