using AutoMapper;
using finance_management.DTOs.ImportCategory;
using finance_management.Interfaces;
using MediatR;

namespace finance_management.Queries.GetCategories
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public GetCategoriesQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(request.ParentId))
            {
                categories = categories.Where(c => c.ParentCode == request.ParentId);
            }

            return _mapper.Map<List<CategoryDto>>(categories.ToList());
        }
    }
}