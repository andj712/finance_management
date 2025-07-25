using finance_management.Interfaces;
using finance_management.Validations.Errors;
using finance_management.Validations.Exceptions;
using MediatR;

namespace finance_management.Commands.SplitTransactions
{
    public class SplitTransactionCommandHandler : IRequestHandler<SplitTransactionCommand, Unit>
    {
        private readonly ISplitRepository _splitRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICategoryRepository _categoryRepository;
        public SplitTransactionCommandHandler(
             ISplitRepository splitRepository,
             ITransactionRepository transactionRepository,
             ICategoryRepository categoryRepository)
        {
            _splitRepository = splitRepository;
            _transactionRepository = transactionRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<Unit> Handle(SplitTransactionCommand request, CancellationToken cancellationToken)
        {
            // prvo da li transakcija postoji
            var transaction = await _transactionRepository.GetByIdAsync(request.TransactionId);
            if (transaction == null)
            {
                throw new BusinessException(new BusinessError
                {
                    Problem = "transaction-not-found",
                    Message = "Transaction not found",
                    Details = $"Transaction with ID {request.TransactionId} does not exist"
                });
            }

            // da li kategorija postoji
            var categoryValidationErrors = new List<ValidationError>();
            foreach (var split in request.Splits)
            {
                var category = await _categoryRepository.GetByCodeAsync(split.CatCode);
                if (category == null)
                {
                    categoryValidationErrors.Add(new ValidationError
                    {
                        Tag = "catcode",
                        Error = ErrorEnum.InvalidValue.ToString(),
                        Message = $"Category with code {split.CatCode} does not exist"
                    });
                }
            }

            if (categoryValidationErrors.Any())
            {
                throw new ValidationException(categoryValidationErrors);
            }

            // da suma splitova bude jednaka iznosu transakcije
            var totalSplitAmount = request.Splits.Sum(s => s.Amount);
            if (totalSplitAmount != transaction.Amount)
            {
                throw new BusinessException(new BusinessError
                {
                    Problem = "invalid-split-amount",
                    Message = "Split amounts must equal transaction amount",
                    Details = $"Total split amount {totalSplitAmount} does not equal transaction amount {transaction.Amount}"
                });
            }

            // izbrisati stare splitove ukoliko postoje
            await _splitRepository.DeleteByTransactionIdAsync(request.TransactionId);

            //kreiranje novih splitova
            var splits = request.Splits.Select(s => new Models.Split
            {
                TransactionId = request.TransactionId,
                CatCode = s.CatCode,
                Amount = s.Amount
            }).ToList();

            //provera da nije dva puta napisana ista kategorija
            var duplicateCodes = request.Splits
                    .GroupBy(s => s.CatCode)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);

            if (duplicateCodes.Any())
            {
                throw new ValidationException(new List<ValidationError>
                        {
                            new ValidationError
                            {
                                Tag = "splits",
                                Error = ErrorEnum.Duplicate.ToString(),
                                Message = $"Duplicate categories found: {string.Join(", ", duplicateCodes)}"
                            }
                        });
                                }
            //provera da li ima dva splita bar
            if (request.Splits.Count()<2)
            {
                            throw new ValidationException(new List<ValidationError>
                {
                    new ValidationError
                    {
                        Tag = "splits",
                        Error = ErrorEnum.Required.ToString(),
                        Message = "At least two split is required"
                    }
                });
                        }
            await _splitRepository.CreateSplitsAsync(splits);

            return Unit.Value;
        }
       
    }
}
