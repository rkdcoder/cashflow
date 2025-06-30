using AutoMapper;
using CashFlow.Entries.Application.Entries.Dto;
using CashFlow.Entries.Domain.Entries.Entities;

namespace CashFlow.Entries.Application.Entries.Mapping
{
    public class EntryMappingProfile : Profile
    {
        public EntryMappingProfile()
        {
            CreateMap<Entry, EntryDto>()
                .ForMember(dest => dest.EntryTypeName, opt => opt.MapFrom(src => src.EntryType != null ? src.EntryType.Name : ""));
        }
    }
}