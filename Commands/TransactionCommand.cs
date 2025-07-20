using finance_management.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace finance_management.Commands
{
    public class TransactionCommand
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Naziv primaoca ne sme biti duži od 255 karaktera")]
        public string BeneficiaryName { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$",
            ErrorMessage = "Datum mora biti u formatu YYYY-MM-DD")]
        public DateTime Date { get; set; }

        [Required]
        public DirectionEnum Direction { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Iznos mora biti pozitivan")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3,
            ErrorMessage = "Currency mora biti 3‑slovni ISO kod")]
        public string Currency { get; set; }

        [Required]
        public TransactionKindEnum Kind { get; set; }

        public MccCodeEnum? MccCode { get; set; }
        
    }
}
