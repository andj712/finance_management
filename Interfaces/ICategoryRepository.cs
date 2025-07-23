using finance_management.Models;
using finance_management.Models.Enums;

namespace finance_management.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByCodeAsync(string code);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> AddAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task<bool> ExistsAsync(string code);
        Task<IEnumerable<Category>> GetByCodesAsync(IEnumerable<string> codes);
        Task SaveChangesAsync();
        Task<SpendingAnalytics> GetSpendingAnalyticsAsync(string catCode, DateTime? startDate, DateTime? endDate, DirectionEnum? direction);

    }
}
