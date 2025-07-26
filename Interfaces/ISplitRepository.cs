using finance_management.Models;

namespace finance_management.Interfaces
{
    public interface ISplitRepository
    {
        Task CreateSplitsAsync(List<Split> splits);
        Task<List<Split>> GetByTransactionIdAsync(string transactionId);
        Task DeleteByTransactionIdAsync(string transactionId);
    }
}
