using CashFlow.Entries.Application.Entries.Dto;
using MediatR;

namespace CashFlow.Entries.Application.Entries.Query
{
    public record GetAllEntryTypesQuery() : IRequest<List<EntryTypeDto>>;
}