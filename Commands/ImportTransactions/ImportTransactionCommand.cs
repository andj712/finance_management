using finance_management.DTOs.ImportTransaction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace finance_management.Commands.ImportTransactions
{
    public class ImportTransactionsCommand : IRequest<Unit>
    {
        
        public IFormFile CsvFile { get; set; }

    }
}
