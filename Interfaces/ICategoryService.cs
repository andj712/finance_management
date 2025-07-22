using finance_management.DTOs.ImportCategory;

namespace finance_management.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> ImportCategoriesAsync(IFormFile file);
        Task<CategoryDto?> GetCategoryAsync(string code);
        Task<List<CategoryDto>> GetAllCategoriesAsync();
    }
}
