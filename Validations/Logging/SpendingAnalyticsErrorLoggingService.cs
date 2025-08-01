using finance_management.Validations.Errors;
using Newtonsoft.Json;

namespace finance_management.Validations.Log
{
    public class SpendingAnalyticsErrorLoggingService
    {
        private readonly string _logDirectory = "Logs";

        public async Task<string> LogValidationErrorsAsync(List<ValidationError> errors)
        {
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }

            var fileName = $"spending-analytics-errors-{DateTime.Now:yyyyMMdd-HHmmss}.json";
            var filePath = Path.Combine(_logDirectory, fileName);

            var logData = new
            {
                Timestamp = DateTime.UtcNow,
                TotalErrors = errors.Count,
                Errors = errors
            };

            var json = JsonConvert.SerializeObject(logData, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, json);

            return fileName;
        }
    }
}