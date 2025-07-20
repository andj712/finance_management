using AutoMapper;
using finance_management.DTOs;
using finance_management.Models;
using finance_management.Models.Enums;

namespace finance_management.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<TransactionCsvDto, Transaction>()
               .ForMember(dest => dest.MccCode, opt => opt.MapFrom(src =>
                 string.IsNullOrWhiteSpace(src.Mcc) ? null : (MccCodeEnum?)Enum.ToObject(typeof(MccCodeEnum), int.Parse(src.Mcc))))
                .ForMember(dest => dest.Kind, opt => opt.MapFrom(src => Enum.Parse<TransactionKindEnum>(src.Kind, true)))
                .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => Enum.Parse<DirectionEnum>(src.Direction, true)))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => decimal.Parse(src.Amount)))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.Parse(src.Date)));
        }
    }
}
