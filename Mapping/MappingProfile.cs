using AutoMapper;
using finance_management.Commands;
using finance_management.Models;
using System.Globalization;

namespace finance_management.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<TransactionCommand, Transaction>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.BeneficiaryName, opt => opt.MapFrom(src => src.BeneficiaryName))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.ParseExact(src.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => src.Direction.ToString().ToLower())) // pretpostavka da Direction u Transaction čeka 'd' ili 'c' malim slovima
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.MccCode, opt => opt.MapFrom(src => (int)src.MccCode)) // nullable int u Transaction, pa cast na int ovde može biti OK
                .ForMember(dest => dest.Kind, opt => opt.MapFrom(src => src.Kind.ToString()))
                ;
        }
    }
}
