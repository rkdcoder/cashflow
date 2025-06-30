using CashFlow.BuildingBlocks.Domain.Caching;
using CashFlow.Consolidations.Application.DailyEntries.Dto;
using CashFlow.Consolidations.Application.DailyEntries.Query;
using CashFlow.Consolidations.Application.DailyEntries.QueryHandler;
using Moq;

namespace CashFlow.Consolidations.Tests.DailyEntries
{
    public class GetDailyEntriesQueryHandlerTests
    {
        private readonly Mock<IRedisCacheService> _redisCacheMock;
        private readonly GetDailyEntriesQueryHandler _handler;

        public GetDailyEntriesQueryHandlerTests()
        {
            _redisCacheMock = new Mock<IRedisCacheService>();
            _handler = new GetDailyEntriesQueryHandler(_redisCacheMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnEntries_WhenCacheHasData()
        {
            // Arrange
            var date = new DateTime(2024, 6, 30);
            var expectedPattern = $"daily-entry_{date:yyyy-MM-dd}_*";

            var expectedEntries = new List<ConsolidationEntryDto>
            {
                new ConsolidationEntryDto { /* sample */ },
                new ConsolidationEntryDto { /* sample */ }
            };

            _redisCacheMock
                .Setup(cache => cache.GetItemsByPatternAsync<ConsolidationEntryDto>(expectedPattern))
                .ReturnsAsync(expectedEntries);

            var query = new GetDailyEntriesQuery(date);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEntries, result);
            _redisCacheMock.Verify(
                cache => cache.GetItemsByPatternAsync<ConsolidationEntryDto>(expectedPattern), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenCacheReturnsNull()
        {
            // Arrange
            var date = new DateTime(2024, 6, 30);
            var expectedPattern = $"daily-entry_{date:yyyy-MM-dd}_*";

            _redisCacheMock
                .Setup(cache => cache.GetItemsByPatternAsync<ConsolidationEntryDto>(expectedPattern))
                .ReturnsAsync((IEnumerable<ConsolidationEntryDto>?)null);

            var query = new GetDailyEntriesQuery(date);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _redisCacheMock.Verify(
                cache => cache.GetItemsByPatternAsync<ConsolidationEntryDto>(expectedPattern), Times.Once);
        }
    }
}