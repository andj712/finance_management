using finance_management.Database;
using finance_management.Models;
using Microsoft.EntityFrameworkCore;

namespace finance_management.Services
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetAllTransactionsAsync(
            string? transactionKind = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int page = 1,
            int pageSize = 10,
            string? sortBy = null,
            string sortOrder = "asc");
    }

    public class TransactionService : ITransactionService
    {
        private readonly PfmDbContext _db;

        public TransactionService(PfmDbContext db)
        {
            _db = db;
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync(
            string? transactionKind = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int page = 1,
            int pageSize = 10,
            string? sortBy = null,
            string sortOrder = "asc")
        {
            var query = _db.Transactions.AsQueryable();

            // Filter by transaction kind
            if (!string.IsNullOrEmpty(transactionKind))
            {
                query = query.Where(t => t.Kind == transactionKind);
            }

            // Filter by date range
            if (startDate.HasValue)
            {
                query = query.Where(t => t.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.Date <= endDate.Value);
            }

            // Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "date":
                        query = sortOrder.ToLower() == "desc"
                            ? query.OrderByDescending(t => t.Date)
                            : query.OrderBy(t => t.Date);
                        break;
                    case "amount":
                        query = sortOrder.ToLower() == "desc"
                            ? query.OrderByDescending(t => t.Amount)
                            : query.OrderBy(t => t.Amount);
                        break;
                    case "beneficiaryname":
                        query = sortOrder.ToLower() == "desc"
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
            var skip = (page - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);

            return await query.ToListAsync();
        }
    }
}