using finance_management.Interfaces;
using finance_management.Validations.Errors;
using Newtonsoft.Json;

namespace finance_management.Validations.Logging
{
    public class ErrorLoggingService : IErrorLoggingService
    {
        private readonly string _logDirectory = "Logs";

        public async Task<string> LogErrorsAsync(List<ValidationError> errors, List<string> skippedRows = null)
        {
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }

            var fileName = $"transaction-query-errors-{DateTime.Now:yyyyMMdd-HHmmss}.json";
            var filePath = Path.Combine(_logDirectory, fileName);

            var logData = new
            {
                Timestamp = DateTime.UtcNow,
                TotalErrors = errors.Count,
                TotalSkippedRows = skippedRows?.Count ?? 0,
                Errors = errors,
                SkippedRows = skippedRows ?? new List<string>()
            };

            var json = JsonConvert.SerializeObject(logData, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, json);

            return fileName;
        }

        public async Task<string> LogBusinessErrorAsync(BusinessError businessError, string context = "")
        {
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }

            var fileName = $"business-error-{DateTime.Now:yyyyMMdd-HHmmss}.json";
            var filePath = Path.Combine(_logDirectory, fileName);

            var logData = new
            {
                Timestamp = DateTime.UtcNow,
                Context = context,
                BusinessError = businessError
            };

            var json = JsonConvert.SerializeObject(logData, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, json);

            return fileName;
        }
    }
}
