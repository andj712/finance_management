using finance_management.DTOs.CategorizeTransaction;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace finance_management.Commands.CategorizeSingleTransaction
{
    public class CategorizeTransactionCommand:IRequest<CategorizeTransactionResult>
    {
        public string TransactionId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category code is required")]
        [JsonPropertyName("cat-code")]
        public string CatCode { get; set; } = string.Empty;
    }
}
