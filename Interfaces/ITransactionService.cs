
using finance_management.Models;

namespace finance_management.Interfaces
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
}
