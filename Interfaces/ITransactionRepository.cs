using finance_management.DTOs.GetTransactions;
using finance_management.Models;
using finance_management.Queries.GetTransactions;

namespace finance_management.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetAllAsync();
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<List<Transaction>> CreateBulkAsync(List<Transaction> transactions);
        Task<Transaction> UpdateAsync(Transaction transaction);
        Task DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        IQueryable<Transaction> Query();
        Task<Transaction?> GetByIdAsync(string id);
        Task UpdateCategoryAsync(string transactionId, string catCode);

        Task<TransactionPagedList> GetTransactionsAsync(GetTransactionsQuery query, CancellationToken cancellationToken = default);
    }
}
