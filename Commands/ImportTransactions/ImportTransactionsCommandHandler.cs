using CsvHelper;
using CsvHelper.Configuration;
using finance_management.Database;
using finance_management.DTOs.ImportTransaction;
using finance_management.Interfaces;
using finance_management.Mapping;
using finance_management.Models;
using finance_management.Models.Enums;
using finance_management.Services;
using finance_management.Validations.Errors;
using finance_management.Validations.Exceptions;
using finance_management.Validations.Log;
using finance_management.Validations.Logging;
using MediatR;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using ValidationException = finance_management.Validations.Exceptions.ValidationException;

namespace finance_management.Commands.ImportTransactions
{
    public class ImportTransactionsCommandHandler : IRequestHandler<ImportTransactionsCommand, Unit>
    {
        private readonly ITransactionRepository _repository;
        private readonly CsvValidationService _csvValidationService;
        private readonly IErrorLoggingService _errorLoggingService;
        private readonly IUnitOfWork _unitOfWork;

        public ImportTransactionsCommandHandler(
            ITransactionRepository repository,
            CsvValidationService csvValidationService,
            IErrorLoggingService errorLoggingService,IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _csvValidationService = csvValidationService;
            _errorLoggingService = errorLoggingService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(ImportTransactionsCommand request, CancellationToken cancellationToken)
        {

            
            try
            {
            using var reader = new StreamReader(request.CsvFile.OpenReadStream());
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null,
                MissingFieldFound = null,
                HeaderValidated = null,
                PrepareHeaderForMatch = args => args.Header.Trim().ToLower().Replace("-", ""),
            };

            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<TransactionCsvMap>();

            // Procitaj sve odjednom i daj mi kao listu objekata TransactionCsvDto
            var records = await csv.GetRecordsAsync<TransactionCsvDto>().ToListAsync(cancellationToken);

            // Validacija hedera
            var headerErrors = _csvValidationService.ValidateHeaders(csv.HeaderRecord ?? Array.Empty<string>());
            if (headerErrors.Any())
                throw new ValidationException(headerErrors);

            var allErrors = new List<ValidationError>();
            var validTransactions = new List<Models.Transaction>();
            var existingIds = await _repository.GetAllIdsAsync(cancellationToken);
            var existingIdsSet = new HashSet<string>(existingIds);
            var csvSeenIds = new HashSet<string>();
            
                
                // validacija svakog reda
            for (int i = 0; i < records.Count; i++)
            {
                var rowNumber = i + 2; // zbog headera
                var (transaction, rowErrors) = _csvValidationService.ValidateAndMapRow(records[i], rowNumber);

                if (rowErrors.Any())
                {
                    allErrors.AddRange(rowErrors);
                    continue;
                }

                    // provera duplikata vec u bazi 
                    //unapredila sam da pre petlje dohvatamo sve IDjeve iz baze jednom i stavljamo ih u existingIdsSet
                    //Unutar petlje ne zovemo više _repository.ExistsAsync(transaction.Id) jer je za svaki red pravio poseban upit 

                    if (existingIdsSet.Contains(transaction!.Id))
                    {
                    var dupErr = new ValidationError
                    {
                        Tag = $"id-row-{rowNumber}",
                        Error = "Duplicate",
                        Message = $"Transaction ID '{transaction.Id}' already exists"
                    };
                    allErrors.Add(dupErr);
                    continue;
                    }
                    //provera duplikata u csv fajlu 
                    if (csvSeenIds.Contains(transaction.Id))
                    {
                        var dupErr = new ValidationError
                        {
                            Tag = $"id-row-{rowNumber}",
                            Error = "Duplicate",
                            Message = $"Transaction ID '{transaction.Id}' is duplicated within the CSV file"
                        };
                        allErrors.Add(dupErr);
                        continue;
                    }
                    csvSeenIds.Add(transaction.Id);
                    validTransactions.Add(transaction!);
            }

                // ukoliko ima gresaka, loguj ih
                if (allErrors.Any())
            {
                await _errorLoggingService.LogErrorsAsync(allErrors, allErrors.Select(e => e.Tag).ToList());
                //ako bas nista nije ubaceno onda moze 400, a ako je bar jedan red ubacen onda moze skip i ok da se vrati ali se loguju greske
                if(!validTransactions.Any()) throw new ValidationException(allErrors);
            }

                //drugi nacin da se ubace transakcije
                //await _repository.AddRangeAsync(validTransactions);

                //await _unitOfWork.SaveChangesAsync(cancellationToken);
                //odlucila sam se za bulk  
                await _repository.CreateBulkAsync(validTransactions);

                return Unit.Value;
            }
            catch (ValidationException)
            {
               
                throw;
            }
            catch (Exception ex)
            {
                
                var businessError = new BusinessError
                {
                    Problem = "import-failed",
                    Message = "Failed to import transactions",
                    Details = ex.Message
                };
                await _errorLoggingService.LogBusinessErrorAsync(businessError, "ImportTransactions");
                throw new BusinessException(businessError);
            }
        }
    }
}

