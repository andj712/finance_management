using MediatR;

namespace finance_management.Commands
{
    public class ImportTransactionsCommand : IRequest
    {
        public string CsvContent { get; set; } = string.Empty;
    }
}
