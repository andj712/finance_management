﻿using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using finance_management.DTOs.ImportCategory;
using finance_management.Interfaces;
using finance_management.Mapping;
using finance_management.Models;
using finance_management.Models.Enums;
using finance_management.Validations.Errors;
using finance_management.Validations.Exceptions;
using finance_management.Validations.Log;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using ValidationException = finance_management.Validations.Exceptions.ValidationException;

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

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                throw new ValidationException(new List<ValidationError>
                    {
                        new ValidationError
                        {
                            Tag = "file",
                            Error = "invalid-format",
                            Message = "File must be a CSV file"
                        }
                    });
            }
            var categories = new List<CategoryDto>();
            var duplicateCodesInFile = new List<string>();
            var updatedCodes = new List<string>();


            using var reader = new StringReader(await ReadFileAsync(file));
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                BadDataFound = null,
                TrimOptions = TrimOptions.Trim
            });
            //prvi red samo procita
            csv.Read();
            //setuj se na prvi red
            csv.ReadHeader();

            var expectedHeaders = new[] { "code", "parent-code", "name" };
            var actualHeaders = csv.HeaderRecord?.Select(h => h.Trim().ToLower()).ToList() ?? new List<string>();

            var missingHeaders = expectedHeaders
                .Where(h => !actualHeaders.Contains(h))
                .ToList();

            if (missingHeaders.Any())
            {
                throw new ValidationException(missingHeaders.Select(h => new ValidationError
                {
                    Tag = "header",
                    Error = ErrorEnum.InvalidFormat.ToString(),
                    Message = $"Missing header: {h}"
                }).ToList());
            }

            //custom mapiranje 
            csv.Context.RegisterClassMap<CategoryCsvMap>();
            var records = csv.GetRecords<CategoryDto>().ToList();
            
            var processedCodes = new HashSet<string>();
            var finalRecords = new List<CategoryDto>();
            // validacija prvo
            var (validRecords, validationErrors) = ValidateCategories(records);


            //  ukloni duplikate u fajlu zadrzi poslednji, loguj samo ako se razlikuju
            var grouped = validRecords
                .GroupBy(r => r.Code)
                .ToList();

            var uniqueRecords = new List<CategoryDto>();

            foreach (var group in grouped)
            {
                var entries = group.ToList();

                if (entries.Count > 1)
                {
                    var distinct = entries
                        .Select(e => new { e.Name, e.ParentCode })
                        .Distinct()
                        .ToList();

                    if (distinct.Count > 1)
                    {
                        duplicateCodesInFile.Add(group.Key); // samo ako se razlikuju
                    }
                }

                uniqueRecords.Add(entries.Last()); // zadrzi poslednji
            }

            // ucitaj postojece kodove i entitete iz baze
            var allExistingCategories = await _categoryRepository.GetAllAsync();
            var existingByCode = allExistingCategories.ToDictionary(c => c.Code);

            // procesiraj svaku kategoriju
            foreach (var dto in uniqueRecords)
            {
                var category = _mapper.Map<Category>(dto);

                if (existingByCode.TryGetValue(dto.Code, out var existingCategory))
                {
                    // Ako su svi podaci isti preskoci
                    if (existingCategory.Name == dto.Name && existingCategory.ParentCode == dto.ParentCode)
                    {
                        continue;
                    }

                    //U suprotnom azuriraj
                    existingCategory.Name = category.Name;
                    existingCategory.ParentCode = category.ParentCode;

                    await _categoryRepository.UpdateAsync(existingCategory);
                    categories.Add(_mapper.Map<CategoryDto>(existingCategory));
                    updatedCodes.Add(dto.Code);
                }
                else
                {
                    // nova kategorija
                    var newCategory = await _categoryRepository.AddAsync(category);
                    categories.Add(_mapper.Map<CategoryDto>(newCategory));
                }
            }

            // loguj promene duplikati u fajlu i azuriranja postojecih
            var loggingService = new CategoryErrorLoggingService();
            await loggingService.LogCategoryErrorsAsync(
                validationErrors,
                duplicateCodesInFile.Concat(updatedCodes).Distinct().ToList()
            );

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

        private (List<CategoryDto> ValidRecords, List<ValidationError> Errors) ValidateCategories(List<CategoryDto> categories)
        {
            var errors = new List<ValidationError>();
            var validRecords = new List<CategoryDto>();

            for (int i = 0; i < categories.Count; i++)
            {
                var category = categories[i];
                var hasError = false;

                if (string.IsNullOrWhiteSpace(category.Code))
                {
                    errors.Add(new ValidationError
                    {
                        Tag = $"categories[{i}].code",
                        Error = "required",
                        Message = "Code is required"
                    });
                    hasError = true;
                }

                if (string.IsNullOrWhiteSpace(category.Name))
                {
                    errors.Add(new ValidationError
                    {
                        Tag = $"categories[{i}].name",
                        Error = "required",
                        Message = "Name is required"
                    });
                    hasError = true;
                }

                if (!hasError)
                {
                    validRecords.Add(category);
                }
            }

            return (validRecords, errors);
        }

        private async Task<string> ReadFileAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        public async Task<Category?> GetByCodeAsync(string code)
        {
            return await _categoryRepository.GetByCodeAsync(code);
        }

        public async Task<SpendingAnalytics> GetSpendingAnalyticsByCategory(string catCode, DateTime? startDate, DateTime? endDate, DirectionEnum? direction)
        {
            return await _categoryRepository.GetSpendingAnalyticsAsync(catCode, startDate, endDate, direction);
        }
        
    }
}
