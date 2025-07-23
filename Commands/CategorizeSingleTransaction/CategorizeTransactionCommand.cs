using System.ComponentModel.DataAnnotations;

namespace finance_management.Commands.CategorizeSingleTransaction
{
    public class CategorizeTransactionCommand
    {
        public string TransactionId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category code is required")]
        public string CatCode { get; set; } = string.Empty;
    }
}
