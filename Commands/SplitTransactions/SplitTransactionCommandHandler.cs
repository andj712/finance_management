using finance_management.Interfaces;
using finance_management.Validations.Errors;
using finance_management.Validations.Exceptions;
using MediatR;
using Microsoft.Office.Interop.Excel;

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
            var errors = new List<ValidationError>();
            if (string.IsNullOrWhiteSpace(request.TransactionId))
            {
                errors.Add(new ValidationError
                {
                    Tag = "id",
                    Error = ErrorEnum.Required.ToString(),
                    Message = "Transaction ID is required"
                });
            }

            if (request.Splits == null || request.Splits.Count()<2)
            {
                errors.Add(new ValidationError
                {
                    Tag = "splits",
                    Error = ErrorEnum.Required.ToString(),
                    Message = "At least two splits are required"
                });
            }
            else
            {
                for (int i = 0; i < request.Splits.Count; i++)
                {
                    var split = request.Splits[i];

                    // Provera da je kategorija uneta
                    if (string.IsNullOrWhiteSpace(split.CatCode))
                    {
                        errors.Add(new ValidationError
                        {
                            Tag = $"splits[{i}].cat-code",
                            Error = ErrorEnum.Required.ToString(),
                            Message = "Category code is required"
                        });
                    }
                    else
                    {
                        split.CatCode = split.CatCode.ToUpper();
                    }

                    // Provera da je iznos pozitivan
                    if (split.Amount <= 0)
                    {
                        errors.Add(new ValidationError
                        {
                            Tag = $"splits[{i}].amount",
                            Error = ErrorEnum.Invalid.ToString(),
                            Message = "Amount must be greater than zero"
                        });
                    }
                }
            }

            if (errors.Any())
                throw new ValidationException(errors);


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
            foreach (var split in request.Splits)
            {
                var category = await _categoryRepository.GetByCodeAsync(split.CatCode);
                if (category == null)
                {
                    throw new BusinessException(new BusinessError
                    {
                        Problem = "category-not-found",
                        Message = "Category not found",
                        Details = $"Category with ID {split.CatCode} does not exist"
                    });
                }
            }


            // da suma splitova bude jednaka iznosu transakcije
            var totalSplitAmount = request.Splits.Sum(s => s.Amount);
            if (Math.Abs(totalSplitAmount - transaction.Amount)>0.01)
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
            
            await _splitRepository.CreateSplitsAsync(splits);

            return Unit.Value;
        }
       
    }
}
