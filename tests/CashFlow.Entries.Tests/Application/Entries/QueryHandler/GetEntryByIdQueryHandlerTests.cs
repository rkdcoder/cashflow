using AutoMapper;
using CashFlow.Entries.Application.Entries.Dto;
using CashFlow.Entries.Application.Entries.Query;
using CashFlow.Entries.Application.Entries.QueryHandler;
using CashFlow.Entries.Domain.Entries.Entities;
using CashFlow.Entries.Domain.Entries.Repositories;
using CashFlow.Shared.Exceptions.Application;
using Moq;

namespace CashFlow.Entries.Tests.Application.Entries.QueryHandler
{
    public class GetEntryByIdQueryHandlerTests
    {
        private readonly Mock<IEntryRepository> _repositoryMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly GetEntryByIdQueryHandler _handler;

        public GetEntryByIdQueryHandlerTests()
        {
            _handler = new GetEntryByIdQueryHandler(
                _repositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Should_Return_EntryDto_When_Entry_Exists()
        {
            // Arrange
            var entryId = Guid.NewGuid();
            var entry = new Entry(123.45m, "Description", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var dto = new EntryDto { Description = "Description", Amount = 123.45m };

            _repositoryMock.Setup(r => r.GetByIdAsync(entryId)).ReturnsAsync(entry);
            _mapperMock.Setup(m => m.Map<EntryDto>(entry)).Returns(dto);

            var query = new GetEntryByIdQuery(entryId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Description, result.Description);
            Assert.Equal(dto.Amount, result.Amount);
        }

        [Fact]
        public async Task Should_Throw_NotFoundException_When_Entry_Not_Found()
        {
            // Arrange
            var entryId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(entryId)).ReturnsAsync((Entry?)null);

            var query = new GetEntryByIdQuery(entryId);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}