namespace finance_management.Models
{
    public class ValidationError
    {
        public string Tag { get; set; } = default!;
        public string Error { get; set; } = default!;
        public string Message { get; set; } = default!;
    }
}
