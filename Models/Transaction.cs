using finance_management.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace finance_management.Models
{
    public class Transaction
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        public string? BeneficiaryName { get; set; } 

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public DirectionEnum Direction { get; set; } 

        [Required]
        [Precision(18, 2)]
        public decimal Amount { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }

        [Required]
        public string Currency { get; set; } = string.Empty;

        
        public MccCodeEnum? MccCode { get; set; }

        [Required]
        public TransactionKindEnum Kind { get; set; }

    }
}
