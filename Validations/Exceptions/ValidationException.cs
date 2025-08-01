using finance_management.Validations.Errors;

namespace finance_management.Validations.Exceptions
{
    public class ValidationException : Exception
    {
        public List<ValidationError> Errors { get; }

        public ValidationException(List<ValidationError> errors) : base("Validation failed")
        {
            Errors = errors;
        }
    }
}
