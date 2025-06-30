using CashFlow.BuildingBlocks.Domain.Caching;
using CashFlow.Consolidations.Application.DailyEntries.Dto;
using CashFlow.Consolidations.Application.DailyEntries.Query;
using CashFlow.Consolidations.Application.DailyEntries.QueryHandler;
using Moq;

namespace CashFlow.Consolidations.Tests.DailyEntries
{
    public class GetAllEntriesQueryHandlerTests
    {
        private readonly Mock<IRedisCacheService> _redisCacheMock;
        private readonly GetAllEntriesQueryHandler _handler;

        public GetAllEntriesQueryHandlerTests()
        {
            _redisCacheMock = new Mock<IRedisCacheService>();
            _handler = new GetAllEntriesQueryHandler(_redisCacheMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnEntries_WhenCacheIsNotNull()
        {
            // Arrange
            var expectedEntries = new List<ConsolidationEntryDto>
            {
                new ConsolidationEntryDto { /* sample */ },
                new ConsolidationEntryDto { /* sample */ }
            };

            _redisCacheMock
                .Setup(cache => cache.GetAllItemsAsync<ConsolidationEntryDto>())
                .ReturnsAsync(expectedEntries);

            // Act
            var result = await _handler.Handle(new GetAllEntriesQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEntries, result);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenCacheIsNull()
        {
            // Arrange
            _redisCacheMock
                .Setup(cache => cache.GetAllItemsAsync<ConsolidationEntryDto>())
                .ReturnsAsync((IEnumerable<ConsolidationEntryDto>?)null);

            // Act
            var result = await _handler.Handle(new GetAllEntriesQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}