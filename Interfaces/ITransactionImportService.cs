using finance_management.Commands;
using finance_management.Validations.Errors;

namespace finance_management.Interfaces
{
    public interface ITransactionImportService
    {
        // Pokusava da importuje CSV fajl i vrati ili listu transakcija ili gresku sa validacijom
       
        Task<(List<TransactionCommand> Transactions, ValidationError ValidationError)> ImportTransactionsAsync(IFormFile file);
    }
}
