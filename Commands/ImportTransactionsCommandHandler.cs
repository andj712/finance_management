using CsvHelper;
using finance_management.Database;
using finance_management.DTOs;
using finance_management.Mapping;
using finance_management.Models;
using finance_management.Models.Enums;
using MediatR;
using System.Globalization;

namespace finance_management.Commands
{
    public class ImportTransactionsCommandHandler : IRequestHandler<ImportTransactionsCommand>
    {
        private readonly PfmDbContext _context;

        public ImportTransactionsCommandHandler(PfmDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ImportTransactionsCommand request, CancellationToken cancellationToken)
        {
            using var reader = new StringReader(request.CsvContent);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<TransactionCsvMap>();
            var records = csv.GetRecords<TransactionCsvDto>();

            foreach (var record in records)
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(record.Id) ||
                    string.IsNullOrWhiteSpace(record.Date) ||
                    string.IsNullOrWhiteSpace(record.Direction) ||
                    string.IsNullOrWhiteSpace(record.Amount) ||
                    string.IsNullOrWhiteSpace(record.Currency) ||
                    string.IsNullOrWhiteSpace(record.Kind))
                    continue; // skip invalid row

                if (!Enum.TryParse<DirectionEnum>(record.Direction, true, out var directionEnum))
                    continue;
                if (!Enum.TryParse<TransactionKindEnum>(record.Kind, true, out var kindEnum))
                    continue;
                if (!DateTime.TryParse(record.Date, out var date))
                    continue;

                date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
                if (!decimal.TryParse(record.Amount, out var amount))
                    continue;

                MccCodeEnum? mccEnum = null;
                if (!string.IsNullOrWhiteSpace(record.Mcc) && int.TryParse(record.Mcc, out var mccCode))
                    mccEnum = Enum.IsDefined(typeof(MccCodeEnum), mccCode) ? (MccCodeEnum?)mccCode : null;

                var transaction = new Transaction
                {
                    Id = record.Id,
                    BeneficiaryName = string.IsNullOrWhiteSpace(record.BeneficiaryName) ? null : record.BeneficiaryName,
                    Date = date,
                    Direction = directionEnum,
                    Amount = amount,
                    Description = string.IsNullOrWhiteSpace(record.Description) ? null : record.Description,
                    Currency = record.Currency,
                    MccCode = mccEnum,
                    Kind = kindEnum
                };

                _context.Transactions.Add(transaction);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        Task IRequestHandler<ImportTransactionsCommand>.Handle(ImportTransactionsCommand request, CancellationToken cancellationToken)
        {
            return Handle(request, cancellationToken);
        }
    }
}
