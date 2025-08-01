using finance_management.Validations.Errors;

namespace finance_management.DTOs.ImportTransaction
{
    public class ImportTransactionsResult
    {
        public int ProcessedCount { get; set; }
        public int SkippedCount { get; set; }
        public int ImportedCount { get; set; }
        public List<ValidationError> ValidationErrors { get; set; } = new();
        public string? LogFileName { get; set; }
    }
}
