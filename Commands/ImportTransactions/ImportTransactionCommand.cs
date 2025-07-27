using finance_management.DTOs.ImportTransaction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace finance_management.Commands.ImportTransactions
{
    public class ImportTransactionsCommand : IRequest<Unit>
    {
        [FromForm(Name = "file")]
        public IFormFile CsvFile { get; set; }

    }
}
