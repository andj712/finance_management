using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace finance_management.DTOs.ImportCategory
{
    public class CategoryDto
    {
        [Required]
        [Name("code")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Name("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("parent-code")]
        [Name("parent-code")]
        public string? ParentCode { get; set; }
    }
}
