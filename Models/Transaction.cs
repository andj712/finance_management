using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace finance_management.Models
{
    public class Transaction
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        public string BeneficiaryName { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Direction { get; set; } = string.Empty; // d or c

        [Required]
        public decimal Amount { get; set; }


        public string? Description { get; set; }

        [Required]
        public string Currency { get; set; } = string.Empty;

        [Column("Mcc")]
        public int? MccCode { get; set; }

        public string? Kind { get; set; }

    }
}
