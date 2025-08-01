using finance_management.DTOs.ImportCategory;
using MediatR;

namespace finance_management.Queries.GetCategories
{
    public class GetCategoriesQuery : IRequest<List<CategoryDto>>
    {
        public string? ParentId { get; set; }
    }
}
