using finance_management.Database;
using finance_management.Interfaces;
using finance_management.Models;
using Microsoft.EntityFrameworkCore;

namespace finance_management.Repository
{
    public class SplitRepository : ISplitRepository
    {
        private readonly PfmDbContext _context;

        public SplitRepository(PfmDbContext context)
        {
            _context = context;
        }

        public async Task CreateSplitsAsync(List<Split> splits)
        {
            await _context.Splits.AddRangeAsync(splits);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Split>> GetByTransactionIdAsync(string transactionId)
        {
            return await _context.Splits
                .Include(s => s.Category)
                .Where(s => s.TransactionId == transactionId)
                .ToListAsync();
        }

        public async Task DeleteByTransactionIdAsync(string transactionId)
        {
            var existingSplits = await _context.Splits
                .Where(s => s.TransactionId == transactionId)
                .ToListAsync();

            if (existingSplits.Any())
            {
                _context.Splits.RemoveRange(existingSplits);
                await _context.SaveChangesAsync();
            }
        }
    }
}
