using finance_management.Database;
using finance_management.Interfaces;
using finance_management.Models;
using finance_management.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace finance_management.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly PfmDbContext _context;

        public CategoryRepository(PfmDbContext context)
        {
            _context = context;
        }

        public async Task<Category?> GetByCodeAsync(string code)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.ChildCategories)
                .ToListAsync();
        }

        public async Task<Category> AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> ExistsAsync(string code)
        {
            return await _context.Categories.AnyAsync(c => c.Code == code);
        }

        public async Task<IEnumerable<Category>> GetByCodesAsync(IEnumerable<string> codes)
        {
            return await _context.Categories
                .Where(c => codes.Contains(c.Code))
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<SpendingAnalytics> GetSpendingAnalyticsAsync(string catCode, DateTime? startDate, DateTime? endDate, DirectionEnum? direction)
        {
            //davanje default vrednosti za start i end date za slucaj da su null
            startDate ??= new DateTime(2021, 1, 1);
            endDate ??= DateTime.Today;

            startDate = startDate.Value.ToUniversalTime();
            endDate = endDate.Value.ToUniversalTime();

            var results = new List<SpendingAnalyticsInCategory>();

            //analitika od onih koje imaju split
            var splitQuery = from t in _context.Transactions
                             join s in _context.Splits on t.Id equals s.TransactionId
                             where t.Date > startDate.Value && t.Date < endDate.Value
                             select new { t, s };

            // dodaj filter za direction ako je naveden
            if (direction != null)
            {
                splitQuery = splitQuery.Where(x => x.t.Direction == direction);
            }

            // dodaj filter za catCode ako je naveden
            if (!string.IsNullOrEmpty(catCode))
            {
                splitQuery = splitQuery.Where(x => x.s.CatCode == catCode);
            }

            var splitResults = await splitQuery
                .GroupBy(x => x.s.CatCode)
                .Select(group => new SpendingAnalyticsInCategory
                {
                    CatCode = group.Key,
                    Amount = Math.Round(group.Sum(x => x.s.Amount), 2),
                    Count = group.Count()
                })
                .ToListAsync();

            results.AddRange(splitResults);

            // analitika od transakcija koje nemaju split
            var transactionIds = await _context.Splits.Select(s => s.TransactionId).Distinct().ToListAsync();

            var transactionQuery = _context.Transactions
                .Where(t => t.Date > startDate.Value && t.Date < endDate.Value
                           && t.CatCode != null
                           && !transactionIds.Contains(t.Id)); // Transactions that don't have splits

            // dodaj direction 
            if (direction != null)
            {
                transactionQuery = transactionQuery.Where(t => t.Direction == direction);
            }

            // dodaj filter za catCode ako je naveden
            if (!string.IsNullOrEmpty(catCode))
            {
                transactionQuery = transactionQuery.Where(t => t.CatCode == catCode);
            }

            var transactionResults = await transactionQuery
                .GroupBy(t => t.CatCode)
                .Select(group => new SpendingAnalyticsInCategory
                {
                    CatCode = group.Key,
                    Amount = Math.Round(group.Sum(t => t.Amount), 2),
                    Count = group.Count()
                })
                .ToListAsync();

            results.AddRange(transactionResults);

            // 3. spoji rezultat ako se ista kategorija pojavljuje u splitovima i obicnim transakcijama
            var finalResults = results
                .GroupBy(r => r.CatCode)
                .Select(group => new SpendingAnalyticsInCategory
                {
                    CatCode = group.Key,
                    Amount = Math.Round(group.Sum(r => r.Amount), 2),
                    Count = group.Sum(r => r.Count)
                })
                .ToList();

            return new SpendingAnalytics
            {
                Groups = finalResults
            };
        }

       
    }
}
