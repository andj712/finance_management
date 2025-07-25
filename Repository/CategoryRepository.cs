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
            startDate ??= DateTime.Today.AddDays(-365);
            endDate ??= DateTime.Today;

            startDate = startDate.Value.ToUniversalTime();
            endDate = endDate.Value.ToUniversalTime();

            var results = new List<SpendingAnalyticsInCategory>();

            // transakcije sa splitovima
            var splitQuery = _context.Transactions
                .Join(_context.Splits, t => t.Id, s => s.TransactionId, (t, s) => new { t, s })
                .Where(x => x.t.Date > startDate.Value && x.t.Date < endDate.Value);

            if (direction != null)
                splitQuery = splitQuery.Where(x => x.t.Direction == direction);

            if (!string.IsNullOrEmpty(catCode))
            {
                // ukljuci splitove koji su te kategorije ili podkategorije od te kategorije
                splitQuery = splitQuery.Where(x =>
                    x.s.CatCode == catCode ||
                    _context.Categories.Any(c => c.Code == x.s.CatCode && c.ParentCode == catCode));
            }
            //prebaci u Listu tipa SpendingAnalyticsInCategory
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

            // Analitika za transakcije bez splitova
            var splitTransactionIds = await _context.Splits.Select(s => s.TransactionId).ToListAsync();//svi id transakcija sa splitovima

            var transactionQuery = _context.Transactions
                .Where(t => t.Date > startDate.Value && t.Date < endDate.Value
                           && t.CatCode != null
                           && !splitTransactionIds.Contains(t.Id));//da nije transakcija sa splitom

            if (direction != null)
                transactionQuery = transactionQuery.Where(t => t.Direction == direction);

            if (!string.IsNullOrEmpty(catCode))
            {
                // da kategorija transakcije bude trazene kategorije ili njene podkategorije
                transactionQuery = transactionQuery.Where(t =>
                    t.CatCode == catCode ||
                    _context.Categories.Any(c => c.Code == t.CatCode && c.ParentCode == catCode));
            }

            //prebaci u Listu tipa SpendingAnalyticsInCategory
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

            // Merge duplicates
            var finalResults = results
                .GroupBy(r => r.CatCode)
                .Select(group => new SpendingAnalyticsInCategory
                {
                    CatCode = group.Key,
                    Amount = Math.Round(group.Sum(r => r.Amount), 2),
                    Count = group.Sum(r => r.Count)
                })
                .ToList();

            return new SpendingAnalytics { Groups = finalResults };

        }

    }
}
