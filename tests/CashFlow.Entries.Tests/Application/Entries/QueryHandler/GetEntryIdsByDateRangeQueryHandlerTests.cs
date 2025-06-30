using CashFlow.Entries.Application.Entries.Query;
using CashFlow.Entries.Application.Entries.QueryHandler;
using CashFlow.Entries.Domain.Entries.Repositories;
using Moq;

namespace CashFlow.Entries.Tests.Application.Entries.QueryHandler
{
    public class GetEntryIdsByDateRangeQueryHandlerTests
    {
        private readonly Mock<IEntryRepository> _repositoryMock = new();
        private readonly GetEntryIdsByDateRangeQueryHandler _handler;

        public GetEntryIdsByDateRangeQueryHandlerTests()
        {
            _handler = new GetEntryIdsByDateRangeQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Should_Return_List_Of_Guids_When_Entries_Exist()
        {
            // Arrange
            var startDate = new DateTime(2025, 1, 1);
            var endDate = new DateTime(2025, 1, 31);

            var expectedIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            _repositoryMock.Setup(r => r.GetIdsByDateRangeAsync(startDate, endDate))
                .ReturnsAsync(expectedIds);

            var query = new GetEntryIdsByDateRangeQuery(startDate, endDate);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedIds.Count, result.Count);
            Assert.Equal(expectedIds, result);
        }

        [Fact]
        public async Task Should_Return_Empty_List_When_No_Entries_Exist()
        {
            // Arrange
            var startDate = new DateTime(2025, 2, 1);
            var endDate = new DateTime(2025, 2, 28);

            _repositoryMock.Setup(r => r.GetIdsByDateRangeAsync(startDate, endDate))
                .ReturnsAsync(new List<Guid>());

            var query = new GetEntryIdsByDateRangeQuery(startDate, endDate);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}