using CashFlow.BuildingBlocks.Domain.Caching;
using CashFlow.Entries.Application.Entries.Command;
using CashFlow.Entries.Application.Entries.CommandHandler;
using CashFlow.Entries.Domain.Entries.Entities;
using CashFlow.Entries.Domain.Entries.Repositories;
using CashFlow.Shared.Abstractions;
using CashFlow.Shared.Exceptions.Application;
using CashFlow.Shared.ValueObjects;
using Moq;

namespace CashFlow.Entries.Tests.Application.Entries.CommandHandler
{
    public class DeleteEntryCommandHandlerTests
    {
        private readonly Mock<IEntryRepository> _entryRepositoryMock = new();
        private readonly Mock<ICurrentUserContext> _currentUserMock = new();
        private readonly Mock<IRedisCacheService> _redisCacheMock = new();

        private readonly DeleteEntryCommandHandler _handler;

        public DeleteEntryCommandHandlerTests()
        {
            _handler = new DeleteEntryCommandHandler(
                _entryRepositoryMock.Object,
                _currentUserMock.Object,
                _redisCacheMock.Object
            );
        }

        [Fact]
        public async Task Should_Delete_Entry_When_User_Has_Manager_Role()
        {
            // Arrange
            var entryId = Guid.NewGuid();
            var entry = new Entry(100, "Test Entry", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            _currentUserMock.Setup(c => c.Roles).Returns(new[] { UserRoles.Manager });
            _entryRepositoryMock.Setup(r => r.GetByIdAsync(entryId)).ReturnsAsync(entry);

            var command = new DeleteEntryCommand(entryId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Entry deleted successfully.", result.Message);
            Assert.True(result.Data);

            _entryRepositoryMock.Verify(r => r.DeleteAsync(entry), Times.Once);
            _redisCacheMock.Verify(c => c.DeleteAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_ForbiddenException_When_User_Is_Not_Manager()
        {
            // Arrange
            var command = new DeleteEntryCommand(Guid.NewGuid());
            _currentUserMock.Setup(c => c.Roles).Returns(new[] { UserRoles.User });

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_NotFoundException_When_Entry_Does_Not_Exist()
        {
            // Arrange
            var command = new DeleteEntryCommand(Guid.NewGuid());
            _currentUserMock.Setup(c => c.Roles).Returns(new[] { UserRoles.Manager });
            _entryRepositoryMock.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync((Entry?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}