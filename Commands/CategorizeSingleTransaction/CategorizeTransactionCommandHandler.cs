using finance_management.DTOs.CategorizeTransaction;
using finance_management.DTOs.ImportTransaction;
using finance_management.Interfaces;
using finance_management.Validations.Errors;
using finance_management.Validations.Log;
using MediatR;
using Newtonsoft.Json;

namespace finance_management.Commands.CategorizeSingleTransaction
{
    public class CategorizeTransactionCommandHandler : IRequestHandler<CategorizeTransactionCommand, CategorizeTransactionResult>
    {
        private readonly ITransactionService _transactionService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategorizeTransactionCommandHandler> _logger;
        private readonly CategoryErrorLoggingService _errorLoggingService;

        public CategorizeTransactionCommandHandler(
            ITransactionService transactionService,
            ICategoryService categoryService,
            ILogger<CategorizeTransactionCommandHandler> logger,
            CategoryErrorLoggingService errorLoggingService)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
            _logger = logger;
            _errorLoggingService = errorLoggingService;
        }

        public async Task<CategorizeTransactionResult> Handle(CategorizeTransactionCommand command, CancellationToken cancellationToken)
        {
            var result = new CategorizeTransactionResult();
            var allErrors = new List<ValidationError>();

            if (string.IsNullOrWhiteSpace(command.TransactionId))
            {
                allErrors.Add(new ValidationError
                {
                    Tag = "id",
                    Error = ErrorEnum.Required.ToString(),
                    Message = "Transaction ID is required"
                });
            }

            if (string.IsNullOrWhiteSpace(command.CatCode))
            {
                allErrors.Add(new ValidationError
                {
                    Tag = "cat-code",
                    Error = ErrorEnum.Required.ToString(),
                    Message = "Category code is required"
                });
            }

            if (allErrors.Any())
            {
                _logger.LogWarning("Validation failed for categorizing transaction");
                result.ValidationErrors = allErrors;
                result.LogFileName = await _errorLoggingService.LogCategoryErrorsAsync(allErrors, new List<string>());
                return result;
            }

            _logger.LogInformation("Categorizing transaction {TransactionId} with category {CatCode}",
                command.TransactionId, command.CatCode);

            var transaction = await _transactionService.GetByIdAsync(command.TransactionId);
            if (transaction == null)
            {
                _logger.LogWarning("Transaction {TransactionId} not found", command.TransactionId);
                result.BusinessError = new BusinessError
                {
                    Problem = "TransactionNotFound",
                    Message = "Transaction not found",
                    Details = $"Transaction with ID '{command.TransactionId}' does not exist."
                };
                return result;
            }

            var category = await _categoryService.GetByCodeAsync(command.CatCode);
            if (category == null)
            {
                _logger.LogWarning("Category {CatCode} not found", command.CatCode);
                result.BusinessError = new BusinessError
                {
                    Problem = "CategoryNotFound",
                    Message = "Category not found",
                    Details = $"Category with code '{command.CatCode}' does not exist."
                };
                return result;
            }

            await _transactionService.UpdateCategoryAsync(command.TransactionId, command.CatCode);

            _logger.LogInformation("Transaction {TransactionId} successfully categorized with {CatCode}",
                command.TransactionId, command.CatCode);

            result.Success = true;
            return result;
        }
    }
}
