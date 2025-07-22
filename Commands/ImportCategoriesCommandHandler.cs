using finance_management.DTOs.ImportCategory;
using finance_management.Interfaces;
using MediatR;

namespace finance_management.Commands
{
    public class ImportCategoriesCommandHandler : IRequestHandler<ImportCategoriesCommand, List<CategoryDto>>
    {
        private readonly ICategoryService _categoryService;

        public ImportCategoriesCommandHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<List<CategoryDto>> Handle(ImportCategoriesCommand request, CancellationToken cancellationToken)
        {
            return await _categoryService.ImportCategoriesAsync(request.File);
        }
    }
}
