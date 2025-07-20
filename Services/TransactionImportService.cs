using CsvHelper;
using finance_management.Commands;
using finance_management.Interfaces;
using finance_management.Validations.Errors;

namespace finance_management.Services
{
    public class TransactionImportService : ITransactionImportService
    {
        private readonly CsvTransactionImporter _csvImporter;

        public TransactionImportService(CsvTransactionImporter csvImporter)
        {
            _csvImporter = csvImporter;
        }

        public async Task<(List<TransactionCommand>, ValidationError)> ImportTransactionsAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                var errorEnum = file == null ? ErrorEnum.Required : ErrorEnum.EmptyFile;

                var error = new ValidationError
                {
                    Errors = new List<Errors>
                {
                    new Errors
                    {
                        Tag = "csv-file",
                        Error = errorEnum,
                        Message = errorEnum.GetEnumDescription()
                    }
                }
                };

                return (null, error);
            }

            List<TransactionCommand> transactionCommands;

            try
            {
                using var stream = file.OpenReadStream();
                transactionCommands = _csvImporter.ImportTransactions(stream);

                if (transactionCommands == null || transactionCommands.Count == 0)
                {
                    var error = new ValidationError
                    {
                        Errors = new List<Errors>
                    {
                        new Errors
                        {
                            Tag = "csv-file",
                            Error = ErrorEnum.EmptyFile,
                            Message = ErrorEnum.EmptyFile.GetEnumDescription()
                        }
                    }
                    };

                    return (null, error);
                }
            }
            catch (CsvHelperException ex)
            {
                var error = new ValidationError
                {
                    Errors = new List<Errors>
                {
                    new Errors
                    {
                        Tag = "csv-file",
                        Error = ErrorEnum.InvalidFormat,
                        Message = $"Pogrešan CSV format: {ex.Message}"
                    }
                }
                };

                return (null, error);
            }

            return (transactionCommands, null);
        }
    }
}
