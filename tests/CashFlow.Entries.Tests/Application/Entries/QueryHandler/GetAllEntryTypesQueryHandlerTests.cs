using AutoMapper;
using CashFlow.Entries.Application.Entries.Dto;
using CashFlow.Entries.Application.Entries.Query;
using CashFlow.Entries.Application.Entries.QueryHandler;
using CashFlow.Entries.Domain.Entries.Entities;
using CashFlow.Entries.Domain.Entries.Repositories;
using Moq;

namespace CashFlow.Entries.Tests.Application.Entries.QueryHandler
{
    public class GetAllEntryTypesQueryHandlerTests
    {
        private readonly Mock<IEntryTypeRepository> _repositoryMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly GetAllEntryTypesQueryHandler _handler;

        public GetAllEntryTypesQueryHandlerTests()
        {
            _handler = new GetAllEntryTypesQueryHandler(
                _repositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Should_Return_EntryTypeDto_List()
        {
            // Arrange
            var types = new List<EntryType>
            {
                new EntryType(Guid.NewGuid(), "REVENUE"),
                new EntryType(Guid.NewGuid(), "EXPENSE")
            };

            var expected = new List<EntryTypeDto>
            {
                new EntryTypeDto { Name = "REVENUE" },
                new EntryTypeDto { Name = "EXPENSE" }
            };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(types);
            _mapperMock.Setup(m => m.Map<List<EntryTypeDto>>(types)).Returns(expected);

            var query = new GetAllEntryTypesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("REVENUE", result[0].Name);
            Assert.Equal("EXPENSE", result[1].Name);
        }
    }
}