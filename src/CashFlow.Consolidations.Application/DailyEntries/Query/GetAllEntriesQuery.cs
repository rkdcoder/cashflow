using CashFlow.Consolidations.Application.DailyEntries.Dto;
using MediatR;

namespace CashFlow.Consolidations.Application.DailyEntries.Query
{
    public record GetAllEntriesQuery() : IRequest<IEnumerable<ConsolidationEntryDto>>;
}