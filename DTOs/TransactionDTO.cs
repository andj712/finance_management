using finance_management.Models.Enums;

namespace finance_management.DTOs
{
    public class TransactionDTO
    {
        public string Id { get; set; }

        public string BeneficiaryName { get; set; }

        public DateTime Date { get; set; }

        public DirectionEnum Direction { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public TransactionKindEnum Kind { get; set; }

        public MccCodeEnum? MccCode { get; set; }
    }
}

