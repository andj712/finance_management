using finance_management.Validations.Errors;
using Newtonsoft.Json;

namespace finance_management.Validations.Log
{
    public class CategoryErrorLoggingService
    {
        private readonly string _logDirectory = "Logs";

        public async Task<string> LogCategoryErrorsAsync(List<ValidationError> errors, List<string> duplicateUpdates)
        {
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }

            var fileName = $"import-category-errors-{DateTime.Now:yyyyMMdd-HHmmss}.json";
            var filePath = Path.Combine(_logDirectory, fileName);

            var logData = new
            {
                Timestamp = DateTime.UtcNow,
                TotalErrors = errors.Count,
                TotalDuplicateUpdates = duplicateUpdates.Count,
                Errors = errors,
                DuplicateUpdates = duplicateUpdates
            };

            var json = JsonConvert.SerializeObject(logData, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, json);

            return fileName;
        }
    }

}
