using finance_management.Database;
using finance_management.Interfaces;
using finance_management.Models;
using finance_management.Models.Enums;
using finance_management.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace finance_management.Services
{
   

    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionService> _logger;
        public TransactionService(ITransactionRepository repo, IUnitOfWork unitOfWork, ILogger<TransactionService> logger)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _logger = logger;
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
            var query = _repo.Query();

            // Filter by transaction kind
            if (!string.IsNullOrEmpty(transactionKind) &&
            Enum.TryParse<TransactionKindEnum>(transactionKind, ignoreCase: true, out var parsedKind))
            {
                query = query.Where(t => t.Kind == parsedKind);
            }

            // Filter by date range
            if (startDate.HasValue)
            {
                var utcStartDate = startDate.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc)
                    : startDate.Value.ToUniversalTime();
                query = query.Where(t => t.Date >= utcStartDate);
            }

            if (endDate.HasValue)
            {
                var utcEndDate = endDate.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc)
                    : endDate.Value.ToUniversalTime();
                query = query.Where(t => t.Date <= utcEndDate);
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

        public async Task<Transaction?> GetByIdAsync(string id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task UpdateCategoryAsync(string transactionId, string catCode)
        {
            await _repo.UpdateCategoryAsync(transactionId, catCode);
        }
    }

}