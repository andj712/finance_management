using finance_management.DTOs.ImportTransaction;
using finance_management.Models;
using finance_management.Models.Enums;
using finance_management.Validations.Errors;
using System.Globalization;

namespace finance_management.Services
{
    public class CsvValidationService
    {
        private readonly string[] _requiredHeaders = {
            "id", "date", "direction", "amount", "currency", "kind"
        };

        public List<ValidationError> ValidateHeaders(string[] headers)
        {
            var errors = new List<ValidationError>();

            foreach (var requiredHeader in _requiredHeaders)
            {
                if (!headers.Any(h => h.Trim().ToLower() == requiredHeader))
                {
                    errors.Add(new ValidationError
                    {
                        Tag = requiredHeader,
                        Error = ErrorEnum.Required.ToString(),
                        Message = $"Required header '{requiredHeader}' is missing"
                    });
                }
            }

            return errors;
        }

        public (Transaction? transaction, List<ValidationError> errors) ValidateAndMapRow(TransactionCsvDto csvDto, int rowNumber)
        {
            var errors = new List<ValidationError>();
            var transaction = new Transaction();

            // Validate ID
            if (string.IsNullOrWhiteSpace(csvDto.Id))
            {
                errors.Add(CreateError("id", "required", "ID is required", rowNumber));
            }
            else
            {
                transaction.Id = csvDto.Id.Trim();
            }

            // Validate Date
            if (string.IsNullOrWhiteSpace(csvDto.Date))
            {
                errors.Add(CreateError("date", "required", "Date is required", rowNumber));
            }
            else if (!DateTime.TryParse(csvDto.Date, out var date))
            {
                errors.Add(CreateError("date", "invalid-format", "Date format is invalid", rowNumber));
            }
            else
            {
                transaction.Date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            }


            // Validate Direction
            if (string.IsNullOrWhiteSpace(csvDto.Direction))
            {
                errors.Add(CreateError("direction", "required", "Direction is required", rowNumber));
            }
            else if (!Enum.TryParse<DirectionEnum>(csvDto.Direction.Trim(), true, out var direction))
            {
                errors.Add(CreateError("direction", "invalid-value", "Direction must be 'd' or 'c'", rowNumber));
            }
            else
            {
                transaction.Direction = direction;
            }

            // Validate Amount
            if (string.IsNullOrWhiteSpace(csvDto.Amount))
            {
                errors.Add(CreateError("amount", "required", "Amount is required", rowNumber));
            }
            else if (!decimal.TryParse(csvDto.Amount, NumberStyles.Currency, CultureInfo.InvariantCulture, out var amount))
            {
                errors.Add(CreateError("amount", "invalid-format", "Amount format is invalid", rowNumber));
            }
            else
            {
                transaction.Amount = (double)amount;
            }

            // Validate Currency
            if (string.IsNullOrWhiteSpace(csvDto.Currency))
            {
                errors.Add(CreateError("currency", "required", "Currency is required", rowNumber));
            }
            else if (csvDto.Currency.Trim().Length != 3)
            {
                errors.Add(CreateError("currency", "invalid-format", "Currency must be 3 characters", rowNumber));
            }
            else
            {
                transaction.Currency = csvDto.Currency.Trim().ToUpper();
            }

            // Validate Kind
            if (string.IsNullOrWhiteSpace(csvDto.Kind))
            {
                errors.Add(CreateError("kind", "required", "Kind is required", rowNumber));
            }
            else if (!Enum.TryParse<TransactionKindEnum>(csvDto.Kind.Trim(), true, out var kind))
            {
                errors.Add(CreateError("kind", "invalid-value", "Invalid transaction kind", rowNumber));
            }
            else
            {
                transaction.Kind = kind;
            }

            // Optional fields
            transaction.BeneficiaryName = string.IsNullOrWhiteSpace(csvDto.BeneficiaryName) ? null : csvDto.BeneficiaryName.Trim();

            if (!string.IsNullOrWhiteSpace(csvDto.Description))
            {
                var description = csvDto.Description.Trim();
                if (description.Length > 50)
                {
                    errors.Add(CreateError("description", "max-length", "Description exceeds maximum allowed length of 50 characters", rowNumber));
                }
                else
                {
                    transaction.Description = description;
                }
            }

            // MCC Code
            if (!string.IsNullOrWhiteSpace(csvDto.Mcc))
            {
                if (int.TryParse(csvDto.Mcc, out var mccInt) && Enum.IsDefined(typeof(MccCodeEnum), mccInt))
                {
                    transaction.MccCode = (MccCodeEnum)mccInt;
                }
                else
                {
                    errors.Add(CreateError("mcc", "invalid-value", "Invalid MCC code", rowNumber));
                }
            }

            return errors.Any() ? (null, errors) : (transaction, errors);
        }

        private ValidationError CreateError(string tag, string error, string message, int rowNumber)
        {
            return new ValidationError
            {
                Tag = $"{tag}-row-{rowNumber}",
                Error = error,
                Message = message
            };
        }
    }
}
