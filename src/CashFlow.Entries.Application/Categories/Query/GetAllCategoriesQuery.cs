using CashFlow.Entries.Application.Categories.Dto;
using MediatR;

namespace CashFlow.Entries.Application.Categories.Query
{
    public record GetAllCategoriesQuery() : IRequest<List<CategoryDto>>;
}