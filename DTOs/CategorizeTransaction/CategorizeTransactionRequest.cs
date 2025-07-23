using System.ComponentModel.DataAnnotations;

namespace finance_management.DTOs.CategorizeTransaction
{
    public class CategorizeTransactionRequest
    {
        [Required(ErrorMessage = "Category code is required")]
        public string CatCode { get; set; } = string.Empty;
    }
}
