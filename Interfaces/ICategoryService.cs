using finance_management.DTOs.ImportCategory;
using finance_management.Models;
using finance_management.Models.Enums;

namespace finance_management.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> ImportCategoriesAsync(IFormFile file);
        Task<CategoryDto?> GetCategoryAsync(string code);
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        
        Task<Category?> GetByCodeAsync(string code);
        Task<SpendingAnalytics> GetSpendingAnalyticsByCategory(string catCode, DateTime? startDate, DateTime? endDate, DirectionEnum? direction);
    }
}
