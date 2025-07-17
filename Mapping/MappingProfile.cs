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
            CreateMap<TransactionCommand, Transaction>();
                
        }
    }
}
