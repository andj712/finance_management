namespace finance_management.Models.Validation
{
    public class ValidationError
    {
        public string Tag { get; set; } = default!;
        public string Error { get; set; } = default!;
        public string Message { get; set; } = default!;
    }
}
