using finance_management.Validations.Errors;

namespace finance_management.DTOs.CategorizeTransaction
{
    public class CategorizeTransactionResult
    {
        public bool Success { get; set; }
        public List<ValidationError> ValidationErrors { get; set; } = new();
        public BusinessError? BusinessError { get; set; }
        public string? LogFileName { get; set; }
    }
}
