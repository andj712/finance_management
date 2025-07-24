using finance_management.Database;
using finance_management.Interfaces;
using finance_management.Models;
using Microsoft.EntityFrameworkCore;

namespace finance_management.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly PfmDbContext _context;

        public TransactionRepository(PfmDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> GetAllAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<Transaction?> GetByIdAsync(string id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<List<Transaction>> CreateBulkAsync(List<Transaction> transactions)
        {
            _context.Transactions.AddRange(transactions);
            await _context.SaveChangesAsync();
            return transactions;
        }

        public async Task<Transaction> UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task DeleteAsync(string id)
        {
            var transaction = await GetByIdAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Transactions.AnyAsync(t => t.Id == id);
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
