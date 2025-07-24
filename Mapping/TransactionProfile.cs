using AutoMapper;
using finance_management.DTOs.ImportTransaction;
using finance_management.Models;
using finance_management.Models.Enums;

namespace finance_management.Mapping
{
    public class TransactionProfile:Profile
    {
        public TransactionProfile()
        {
            CreateMap<TransactionCsvDto, Transaction>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Trim()))
                .ForMember(dest => dest.BeneficiaryName, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.BeneficiaryName) ? null : src.BeneficiaryName.Trim()))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src =>
                    DateTime.SpecifyKind(DateTime.Parse(src.Date), DateTimeKind.Utc)))
                .ForMember(dest => dest.Direction, opt => opt.MapFrom(src =>
                    Enum.Parse<DirectionEnum>(src.Direction.Trim(), true)))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src =>
                    decimal.Parse(src.Amount, System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.Description) ? null : src.Description.Trim()))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency.Trim().ToUpper()))
                .ForMember(dest => dest.MccCode, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.Mcc) ? (MccCodeEnum?)null :
                    (MccCodeEnum)int.Parse(src.Mcc)))
                .ForMember(dest => dest.Kind, opt => opt.MapFrom(src =>
                    Enum.Parse<TransactionKindEnum>(src.Kind.Trim(), true)));
        }
    }
}
