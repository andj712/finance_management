using finance_management.Validations.Errors;

namespace finance_management.Interfaces
{
    public interface IErrorLoggingService
    {
        Task<string> LogErrorsAsync(List<ValidationError> errors, List<string> skippedRows = null);
        Task<string> LogBusinessErrorAsync(BusinessError businessError, string context = "");
    }
}
