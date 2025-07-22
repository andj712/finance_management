using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using finance_management.DTOs.ImportCategory;
using finance_management.Interfaces;
using finance_management.Mapping;
using finance_management.Models;
using finance_management.Validations.Errors;
using finance_management.Validations.Exceptions;
using System.Globalization;

namespace finance_management.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<List<CategoryDto>> ImportCategoriesAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null");

            var categories = new List<CategoryDto>();

            using var reader = new StringReader(await ReadFileAsync(file));
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                BadDataFound = null,
                TrimOptions = TrimOptions.Trim
            });
            //custom mapiranje 
            csv.Context.RegisterClassMap<CategoryCsvMap>();
            var records = csv.GetRecords<CategoryDto>().ToList();

            // validacija prvo
            ValidateCategories(records);

            // provera duplikata
            var duplicatesInFile = records.GroupBy(r => r.Code)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicatesInFile.Any())
            {
                throw new BusinessException(new BusinessError
                {
                    Problem = "duplicate-categories",
                    Message = "Duplicate categories found in file",
                    Details = $"Categories with codes: {string.Join(", ", duplicatesInFile)} appear multiple times in the file"
                });
            }

            // procesiranje kategorija
            foreach (var categoryDto in records)
            {
                var existingCategory = await _categoryRepository.GetByCodeAsync(categoryDto.Code);
                var category = _mapper.Map<Category>(categoryDto);

                if (existingCategory != null)
                {
                    //azuriranje postojece kategorije
                    existingCategory.Name = category.Name;
                    existingCategory.ParentCode = category.ParentCode;
                    await _categoryRepository.UpdateAsync(existingCategory);
                    categories.Add(_mapper.Map<CategoryDto>(existingCategory));

                }
                else
                {
                    // dodavanje nove kategorije
                    var newCategory = await _categoryRepository.AddAsync(category);
                    categories.Add(_mapper.Map<CategoryDto>(newCategory));
                }
            }

            return categories;
        }

        public async Task<CategoryDto?> GetCategoryAsync(string code)
        {
            var category = await _categoryRepository.GetByCodeAsync(code);
            return category != null ? _mapper.Map<CategoryDto>(category) : null;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        private void ValidateCategories(List<CategoryDto> categories)
        {
            var errors = new List<ValidationError>();

            for (int i = 0; i < categories.Count; i++)
            {
                var category = categories[i];

                if (string.IsNullOrWhiteSpace(category.Code))
                {
                    errors.Add(new ValidationError
                    {
                        Tag = $"categories[{i}].code",
                        Error = "required",
                        Message = "Code is required"
                    });
                }

                if (string.IsNullOrWhiteSpace(category.Name))
                {
                    errors.Add(new ValidationError
                    {
                        Tag = $"categories[{i}].name",
                        Error = "required",
                        Message = "Name is required"
                    });
                }
            }

            if (errors.Any())
            {
                throw new Validations.Exceptions.ValidationException(errors);
            }
        }

        private async Task<string> ReadFileAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}
