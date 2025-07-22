using finance_management.DTOs.ImportCategory;
using MediatR;

namespace finance_management.Commands
{
    public class ImportCategoriesCommand : IRequest<List<CategoryDto>>
    {
        public IFormFile File { get; set; } = null!;
    }
}
