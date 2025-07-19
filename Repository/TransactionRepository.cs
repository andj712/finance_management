using finance_management.Database;
using finance_management.Interfaces;
using finance_management.Models;

namespace finance_management.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly PfmDbContext _context;

        public TransactionRepository(PfmDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Transaction tx)
        {
            await _context.Transactions.AddAsync(tx);
        }

        public IQueryable<Transaction> Query()
        {
            return _context.Transactions.AsQueryable();
        }
    }
}
