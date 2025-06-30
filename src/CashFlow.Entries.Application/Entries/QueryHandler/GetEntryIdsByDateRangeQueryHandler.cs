using CashFlow.Entries.Application.Entries.Query;
using CashFlow.Entries.Domain.Entries.Repositories;
using MediatR;

namespace CashFlow.Entries.Application.Entries.QueryHandler
{
    public class GetEntryIdsByDateRangeQueryHandler : IRequestHandler<GetEntryIdsByDateRangeQuery, List<Guid>>
    {
        private readonly IEntryRepository _repository;

        public GetEntryIdsByDateRangeQueryHandler(IEntryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Guid>> Handle(GetEntryIdsByDateRangeQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetIdsByDateRangeAsync(request.StartDate, request.EndDate);
        }
    }
}