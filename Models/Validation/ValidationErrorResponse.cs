namespace finance_management.Models.Validation
{
    public class ValidationErrorResponse
    {
        public List<ValidationError> Errors { get; set; } = default!;
    }
}
