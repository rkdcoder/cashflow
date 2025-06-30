using AutoMapper;
using CashFlow.Entries.Application.Entries.Dto;
using CashFlow.Entries.Domain.Entries.Entities;

namespace CashFlow.Entries.Application.Entries.Mapping
{
    public class EntryTypeMappingProfile : Profile
    {
        public EntryTypeMappingProfile()
        {
            CreateMap<EntryType, EntryTypeDto>();
        }
    }
}