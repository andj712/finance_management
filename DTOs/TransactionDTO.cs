using finance_management.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace finance_management.DTOs
{
    public class TransactionDTO
    {
        [Required]
        [StringLength(10, MinimumLength = 1)]
        public string Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 1)]
        public string BeneficiaryName { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [EnumDataType(typeof(DirectionEnum))]
        public DirectionEnum Direction { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency must be a 3-letter code (e.g., USD, EUR)")]
        public string Currency { get; set; }

        [Required]
        [EnumDataType(typeof(TransactionKindEnum))]
        public TransactionKindEnum Kind { get; set; }

        [EnumDataType(typeof(MccCodeEnum))]
        public MccCodeEnum? MccCode { get; set; }
    }
}