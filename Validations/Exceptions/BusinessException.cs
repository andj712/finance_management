using finance_management.Validations.Errors;

namespace finance_management.Validations.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessError Error { get; }

        public BusinessException(BusinessError error) : base(error.Message)
        {
            Error = error;
        }
    }
}
