using finance_management.Validations.Errors;
using Newtonsoft.Json;

namespace finance_management.Services
{
    public class ErrorLoggingService
    {
        private readonly string _logDirectory = "Logs";

        public async Task<string> LogErrorsAsync(List<ValidationError> errors, List<string> skippedRows)
        {
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }

            var fileName = $"import-errors-{DateTime.Now:yyyyMMdd-HHmmss}.json";
            var filePath = Path.Combine(_logDirectory, fileName);

            var logData = new
            {
                Timestamp = DateTime.UtcNow,
                TotalErrors = errors.Count,
                TotalSkippedRows = skippedRows.Count,
                Errors = errors,
                SkippedRows = skippedRows
            };

            var json = JsonConvert.SerializeObject(logData, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, json);

            return fileName;
        }
    }
}
