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
            startDate ??=new DateTime(2021, 1, 1);
            endDate ??= DateTime.Today;


            startDate = startDate.Value.ToUniversalTime();
            

            endDate = endDate.Value.ToUniversalTime();

            //filtrira transakcije po datumu i kategoriji
            var transactionsQuery = _context.Transactions.AsQueryable().Where(
                t => t.Date > startDate.Value && t.Date < endDate.Value && t.CatCode != null);
            //opcioni filter, provera dal je null 
            if (!string.IsNullOrEmpty(catCode))
            {
                transactionsQuery = transactionsQuery.Where(t => t.CatCode == catCode);
            }
            //opcioni filter
            if (direction != null)
            {
                transactionsQuery = transactionsQuery.Where(t => t.Direction == direction);
            }
            var groupedTransactions =await transactionsQuery
                .GroupBy(t => t.CatCode)
                .Select(group => new SpendingAnalyticsInCategory
                {
                    CatCode = group.Key,
                    Amount = (double)Math.Round(group.Sum(t => t.Amount), 2),
                    Count = group.Count()
                })
                .ToListAsync();

            return new SpendingAnalytics
            {
                Groups = groupedTransactions
            };
        }

       
    }
}
