using finance_management.Database;
using finance_management.DTOs.GetTransactions;
using finance_management.Interfaces;
using finance_management.Models;
using finance_management.Models.Enums;
using finance_management.Validations.Errors;
using finance_management.Validations.Exceptions;
using finance_management.Validations.Logging;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace finance_management.Queries.GetTransactions
{
    public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, TransactionPagedList>
    {
        private readonly ITransactionRepository _repository;
        private readonly IErrorLoggingService _errorLoggingService;
        private static readonly string[] AllowedSortFields = {
        "id","date","amount","beneficiary-name","description",
        "currency","mcc-code","kind","cat-code","direction"
        };
        public GetTransactionsQueryHandler(ITransactionRepository repository,ErrorLoggingService errorLoggingService)
        {
            _repository = repository;
            _errorLoggingService = errorLoggingService;
        }

        public async Task<TransactionPagedList> Handle(
       GetTransactionsQuery request,
       CancellationToken cancellationToken)
        {
            // 1) Validiraj request
            var errors = Validate(request);
            if (errors.Any())
            {
                // 2) Loguj i baci exception
                await _errorLoggingService.LogErrorsAsync(errors);
                throw new ValidationException(errors);
            }

            // 3) Pozovi repozitorijum i vrati rezultat
            try
            {
                return await _repository.GetTransactionsAsync(request, cancellationToken);
            }
            catch (BusinessException ex)
            {
                await _errorLoggingService.LogBusinessErrorAsync(ex.Error, nameof(GetTransactionsQuery));
                throw;
            }
        }

        private List<ValidationError> Validate(GetTransactionsQuery q)
        {
            var errors = new List<ValidationError>();

            //transaction kind
            if (!string.IsNullOrWhiteSpace(q.TransactionKind))
            {
                foreach (var raw in q.TransactionKind
                             .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    if (!Enum.TryParse<TransactionKindEnum>(raw, true, out _))
                    {
                        errors.Add(new ValidationError
                        {
                            Tag = "transaction-kind",
                            Error = ErrorEnum.InvalidValue.ToString(),
                            Message = $"Unsupported kind '{raw}'."
                        });
                    }
                }
            }

            //sort-by
            var fields = (q.SortBy ?? "date")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(f => f.ToLower());
            foreach (var f in fields)
            {
                if (!AllowedSortFields.Contains(f))
                {
                    errors.Add(new ValidationError
                    {
                        Tag = "sort-by",
                        Error = ErrorEnum.InvalidValue.ToString(),
                        Message = $"Unsupported sort field '{f}'."
                    });
                }
            }

            // page page-size range
            if (q.Page <= 0)
            {
                errors.Add(new ValidationError
                {
                    Tag = "page",
                    Error = ErrorEnum.InvalidValue.ToString(),
                    Message = "Page must be >= 1."
                });
            }
            if (q.PageSize < 1 || q.PageSize > 100)
            {
                errors.Add(new ValidationError
                {
                    Tag = "page-size",
                    Error = ErrorEnum.InvalidValue.ToString(),
                    Message = "PageSize must be between 1 and 100."
                });
            }

            return errors;
        }
    }
}

