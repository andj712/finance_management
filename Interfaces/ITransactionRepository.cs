using finance_management.Models;

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
    }
}
