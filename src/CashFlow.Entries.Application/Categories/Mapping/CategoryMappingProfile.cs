using AutoMapper;
using CashFlow.Entries.Application.Categories.Dto;
using CashFlow.Entries.Domain.Entries.Entities;

namespace CashFlow.Entries.Application.Categories.Mapping
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.EntryTypeName, opt => opt.MapFrom(src => src.EntryType != null ? src.EntryType.Name : ""));
        }
    }
}