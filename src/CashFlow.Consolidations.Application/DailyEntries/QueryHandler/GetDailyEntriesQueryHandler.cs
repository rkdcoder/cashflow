using CashFlow.BuildingBlocks.Domain.Caching;
using CashFlow.Consolidations.Application.DailyEntries.Dto;
using CashFlow.Consolidations.Application.DailyEntries.Query;
using MediatR;

namespace CashFlow.Consolidations.Application.DailyEntries.QueryHandler
{
    public class GetDailyEntriesQueryHandler : IRequestHandler<GetDailyEntriesQuery, IEnumerable<ConsolidationEntryDto>>
    {
        private readonly IRedisCacheService _redisCache;

        public GetDailyEntriesQueryHandler(IRedisCacheService redisCache)
        {
            _redisCache = redisCache;
        }

        public async Task<IEnumerable<ConsolidationEntryDto>> Handle(GetDailyEntriesQuery request, CancellationToken cancellationToken)
        {
            var pattern = $"daily-entry_{request.Date:yyyy-MM-dd}_*";
            var entries = await _redisCache.GetItemsByPatternAsync<ConsolidationEntryDto>(pattern) ?? new List<ConsolidationEntryDto>();
            return entries;
        }
    }
}