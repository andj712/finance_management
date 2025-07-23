using finance_management.DTOs.ImportCategory;
using finance_management.Models;

namespace finance_management.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> ImportCategoriesAsync(IFormFile file);
        Task<CategoryDto?> GetCategoryAsync(string code);
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        
        Task<Category?> GetByCodeAsync(string code);
       

    }
}
