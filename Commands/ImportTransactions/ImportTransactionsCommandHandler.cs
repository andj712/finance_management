using CsvHelper;
using finance_management.Database;
using finance_management.DTOs.ImportTransaction;
using finance_management.Interfaces;
using finance_management.Mapping;
using finance_management.Models;
using finance_management.Models.Enums;
using finance_management.Services;
using finance_management.Validations.Errors;
using finance_management.Validations.Log;
using finance_management.Validations.Logging;
using MediatR;
using System.Globalization;

namespace finance_management.Commands.ImportTransactions
{
    public class ImportTransactionsCommandHandler : IRequestHandler<ImportTransactionsCommand, ImportTransactionsResult>
    {
        private readonly ITransactionRepository _repository;
        private readonly CsvProcessingService _csvProcessingService;
        private readonly CsvValidationService _csvValidationService;
        private readonly ErrorLoggingService _errorLoggingService;

        public ImportTransactionsCommandHandler(
            ITransactionRepository repository,
            CsvProcessingService csvProcessingService,
            CsvValidationService csvValidationService,
            ErrorLoggingService errorLoggingService)
        {
            _repository = repository;
            _csvProcessingService = csvProcessingService;
            _csvValidationService = csvValidationService;
            _errorLoggingService = errorLoggingService;
        }

        public async Task<ImportTransactionsResult> Handle(ImportTransactionsCommand request, CancellationToken cancellationToken)
        {
            var result = new ImportTransactionsResult();
            var allErrors = new List<ValidationError>();
            var skippedRows = new List<string>();

            // validacija fajla
            if (request.CsvFile == null || request.CsvFile.Length == 0)
            {
                result.ValidationErrors.Add(new ValidationError
                {
                    Tag = "file",
                    Error = "required",
                    Message = "CSV file is required"
                });
                return result;
            }

            if (!request.CsvFile.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                result.ValidationErrors.Add(new ValidationError
                {
                    Tag = "file",
                    Error = "invalid-format",
                    Message = "File must be a CSV file"
                });
                return result;
            }

            try
            {
                using var stream = request.CsvFile.OpenReadStream();
                var (records, headers) = await _csvProcessingService.ParseCsvAsync(stream);

                // validacija zaglavlja
                var headerErrors = _csvValidationService.ValidateHeaders(headers);
                if (headerErrors.Any())
                {
                    result.ValidationErrors.AddRange(headerErrors);
                    return result;
                }

                var validTransactions = new List<Transaction>();
                var rowNumber = 1;

                foreach (var csvDto in records)
                {
                    rowNumber++;
                    result.ProcessedCount++;

                    var (transaction, rowErrors) = _csvValidationService.ValidateAndMapRow(csvDto, rowNumber);

                    if (rowErrors.Any())
                    {
                        allErrors.AddRange(rowErrors);
                        skippedRows.Add($"Row {rowNumber}: {string.Join(", ", rowErrors.Select(e => e.Message))}");
                        result.SkippedCount++;
                        continue;
                    }

                    if (transaction != null)
                    {
                        // provera da li vec postoji transakcija sa istim id-om
                        if (await _repository.ExistsAsync(transaction.Id))
                        {
                            allErrors.Add(new ValidationError
                            {
                                Tag = $"id-row-{rowNumber}",
                                Error = "duplicate",
                                Message = $"Transaction with ID {transaction.Id} already exists"
                            });
                            skippedRows.Add($"Row {rowNumber}: Duplicate transaction ID {transaction.Id}");
                            result.SkippedCount++;
                            continue;
                        }

                        validTransactions.Add(transaction);
                    }
                }

                // import validnih transakcija
                if (validTransactions.Any())
                {
                    await _repository.CreateBulkAsync(validTransactions);
                    result.ImportedCount = validTransactions.Count;
                }

                // logovanje gresaka
                if (allErrors.Any() || skippedRows.Any())
                {
                    result.LogFileName = await _errorLoggingService.LogErrorsAsync(allErrors, skippedRows);
                }

                result.ValidationErrors = allErrors;
                return result;
            }
            catch (Exception ex)
            {
                result.ValidationErrors.Add(new ValidationError
                {
                    Tag = "processing",
                    Error = "internal-error",
                    Message = $"Error processing CSV file: {ex.Message}"
                });
                return result;
            }
        }
    }
}
