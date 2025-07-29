using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace finance_management.DTOs.CategorizeTransaction
{
    public class CategorizeTransactionRequest
    {
        [Required(ErrorMessage = "Category code is required")]
        [JsonPropertyName("catcode")] 
        public string CatCode { get; set; } = string.Empty;
    }
}
