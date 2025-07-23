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

        public SpendingAnalytics GetSpendingAnalytics(string catCode, DateTime? startDate, DateTime? endDate, DirectionEnum? direction)
        {
            throw new NotImplementedException();
        }
    }
}
