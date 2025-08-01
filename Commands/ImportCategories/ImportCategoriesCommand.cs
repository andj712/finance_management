using finance_management.DTOs.ImportCategory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace finance_management.Commands.ImportCategories
{
    public class ImportCategoriesCommand : IRequest<List<CategoryDto>>
    {
        [FromForm]
        public IFormFile File { get; set; } = null!;
    }
}
