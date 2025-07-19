using finance_management.Models;

namespace finance_management.Interfaces
{
    public interface ITransactionRepository
    {
        IQueryable<Transaction> Query();
    }
}
