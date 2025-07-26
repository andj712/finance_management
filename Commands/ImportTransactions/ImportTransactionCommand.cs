using finance_management.DTOs.ImportTransaction;
using MediatR;

namespace finance_management.Commands.ImportTransactions
{
    public class ImportTransactionsCommand : IRequest<ImportTransactionsResult>
    {
        public IFormFile CsvFile { get; set; }

        public ImportTransactionsCommand(IFormFile csvFile)
        {
            CsvFile = csvFile;
        }
    }
}
