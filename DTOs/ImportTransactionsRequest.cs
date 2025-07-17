using System.ComponentModel.DataAnnotations;

namespace finance_management.DTOs
{
    public class ImportTransactionsRequest
    {
        [Required]
        public IFormFile File { get; set; } = default!;
    }
}
