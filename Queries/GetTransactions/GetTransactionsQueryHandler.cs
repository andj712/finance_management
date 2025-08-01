using AutoMapper;
using finance_management.Database;
using finance_management.DTOs.GetTransactions;
using finance_management.Interfaces;
using finance_management.Models;
using finance_management.Models.Enums;
using finance_management.Repository;
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
        private readonly IMapper _mapper;
        private static readonly string[] AllowedSortFields = {
        "id","date","amount","beneficiary-name","description",
        "currency","mcc-code","kind","cat-code","direction"
        };
        public GetTransactionsQueryHandler(ITransactionRepository repository,ErrorLoggingService errorLoggingService,IMapper mapper)
        {
            _repository = repository;
            _errorLoggingService = errorLoggingService;
            _mapper = mapper;
        }

        public async Task<TransactionPagedList> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            var errors = Validate(request);
            if (errors.Any())
            {
                await _errorLoggingService.LogErrorsAsync(errors);
                throw new ValidationException(errors);
            }

            // repository vraca TransactionPagedList 
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

        private List<ValidationError> Validate(GetTransactionsQuery request)
        {
            var errors = new List<ValidationError>();

            // Validacija transaction kinds
            if (!string.IsNullOrWhiteSpace(request.TransactionKind))
            {
                var kinds = request.TransactionKind.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var kind in kinds)
                {
                    if (!Enum.TryParse<TransactionKindEnum>(kind, true, out _))
                    {
                        errors.Add(new ValidationError
                        {
                            Tag = "transaction-kind",
                            Error = ErrorEnum.InvalidValue.ToString(),
                            Message = $"Unsupported transaction kind '{kind}'. Valid values: {string.Join(", ", Enum.GetNames<TransactionKindEnum>())}"
                        });
                    }
                }
            }

           
            

            // Validiraj sort 
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                var sortFields = request.SortBy.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var field in sortFields)
                {
                    if (!AllowedSortFields.Contains(field.ToLower()))
                    {
                        errors.Add(new ValidationError
                        {
                            Tag = "sort-by",
                            Error = ErrorEnum.InvalidValue.ToString(),
                            Message = $"Unsupported sort field '{field}'. Valid fields: {string.Join(", ", AllowedSortFields)}"
                        });
                    }
                }
            }

            // Validacija paginacije
            if (request.Page <= 0)
            {
                errors.Add(new ValidationError
                {
                    Tag = "page",
                    Error = ErrorEnum.InvalidValue.ToString(),
                    Message = "Page must be >= 1"
                });
            }

            if (request.PageSize < 1 || request.PageSize > 100)
            {
                errors.Add(new ValidationError
                {
                    Tag = "page-size",
                    Error = ErrorEnum.InvalidValue.ToString(),
                    Message = "Page size must be between 1 and 100"
                });
            }

            if (request.StartDate.HasValue)
            {
                if (request.StartDate.Value > DateTime.Today)
                {
                    errors.Add(new ValidationError
                    {
                        Tag = "start-date",
                        Error = ErrorEnum.InvalidValue.ToString(),
                        Message = "Start date cannot be in the future"
                    });
                }
            }
            if (request.EndDate.HasValue)
            {
                if (request.EndDate.Value < new DateTime(2010, 1, 1))
                {
                    errors.Add(new ValidationError
                    {
                        Tag = "end-date",
                        Error = ErrorEnum.InvalidValue.ToString(),
                        Message = "End date is too far in the past"
                    });
                }
            }

            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                if (request.StartDate.Value > request.EndDate.Value)
                {
                    errors.Add(new ValidationError
                    {
                        Tag = "date-range",
                        Error = ErrorEnum.InvalidValue.ToString(),
                        Message = "Start date cannot be after end date"
                    });
                }

                var range = request.EndDate.Value - request.StartDate.Value;
                if (range.TotalDays > 365)
                {
                    errors.Add(new ValidationError
                    {
                        Tag = "date-range",
                        Error = ErrorEnum.InvalidValue.ToString(),
                        Message = "Date range cannot exceed 365 days"
                    });
                }
            }


            return errors;
        }
    }
}

