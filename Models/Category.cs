using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace finance_management.Models
{
    public class Category
    {
        [Key]
        [Required]
        [Name("code")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Name("name")]
        public string Name { get; set; } = string.Empty;

        [Name("parent-code")]
        [JsonPropertyName("parent-code")]
        public string? ParentCode { get; set; }

        // Navigation properties
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual ICollection<Split> Splits { get; set; } = new List<Split>();
        public virtual Category? ParentCategory { get; set; }
        public virtual ICollection<Category> ChildCategories { get; set; } = new List<Category>();

    }
}
