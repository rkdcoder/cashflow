using CashFlow.BuildingBlocks.Domain.Caching;
using CashFlow.Consolidations.Application.DailyEntries.Dto;
using CashFlow.Consolidations.Application.DailyEntries.Query;
using MediatR;

namespace CashFlow.Consolidations.Application.DailyEntries.QueryHandler
{
    public class GetAllEntriesQueryHandler : IRequestHandler<GetAllEntriesQuery, IEnumerable<ConsolidationEntryDto>>
    {
        private readonly IRedisCacheService _redisCache;

        public GetAllEntriesQueryHandler(IRedisCacheService redisCache)
        {
            _redisCache = redisCache;
        }

        public async Task<IEnumerable<ConsolidationEntryDto>> Handle(GetAllEntriesQuery request, CancellationToken cancellationToken)
        {
            var entries = await _redisCache.GetAllItemsAsync<ConsolidationEntryDto>() ?? new List<ConsolidationEntryDto>();
            return entries;
        }
    }
}