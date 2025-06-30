using MediatR;

namespace CashFlow.Entries.Application.Entries.Query
{
    public record GetEntryIdsByDateRangeQuery(
        DateTime StartDate,
        DateTime EndDate
    ) : IRequest<List<Guid>>;
}