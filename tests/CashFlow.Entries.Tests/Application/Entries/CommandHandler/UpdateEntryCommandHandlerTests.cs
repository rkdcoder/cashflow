using CashFlow.BuildingBlocks.Domain.Caching;
using CashFlow.Entries.Application.Entries.Command;
using CashFlow.Entries.Application.Entries.CommandHandler;
using CashFlow.Entries.Domain.Categories.Repositories;
using CashFlow.Entries.Domain.Entries.Entities;
using CashFlow.Entries.Domain.Entries.Repositories;
using CashFlow.Shared.Abstractions;
using CashFlow.Shared.Exceptions.Application;
using CashFlow.Shared.ValueObjects;
using Moq;

namespace CashFlow.Entries.Tests.Application.Entries.CommandHandler
{
    public class UpdateEntryCommandHandlerTests
    {
        private readonly Mock<IEntryRepository> _entryRepositoryMock = new();
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
        private readonly Mock<ICurrentUserContext> _currentUserMock = new();
        private readonly Mock<IRedisCacheService> _redisCacheMock = new();

        private readonly UpdateEntryCommandHandler _handler;

        public UpdateEntryCommandHandlerTests()
        {
            _handler = new UpdateEntryCommandHandler(
                _entryRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _currentUserMock.Object,
                _redisCacheMock.Object
            );
        }

        [Fact]
        public async Task Should_Update_Entry_When_Valid_Request()
        {
            // Arrange
            var entryId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var entryTypeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new UpdateEntryCommand(entryId, 200, "Updated", categoryId);

            var entry = new Entry(100, "Old", entryTypeId, categoryId, userId);
            var category = new Category(categoryId, "UpdatedCat", entryTypeId);

            _currentUserMock.Setup(u => u.Roles).Returns(new[] { UserRoles.Manager });
            _currentUserMock.Setup(u => u.Subject).Returns(userId.ToString());

            _entryRepositoryMock.Setup(r => r.GetByIdAsync(entryId)).ReturnsAsync(entry);
            _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Entry updated successfully.", result.Message);

            _entryRepositoryMock.Verify(r => r.UpdateAsync(entry), Times.Once);
            _redisCacheMock.Verify(c => c.AddOrUpdateAsync(It.IsAny<string>(), entry), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_ForbiddenException_When_User_Not_Manager()
        {
            var command = new UpdateEntryCommand(Guid.NewGuid(), 100, "x", Guid.NewGuid());
            _currentUserMock.Setup(u => u.Roles).Returns(new[] { UserRoles.User });

            await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_BadRequestException_When_Amount_Is_Invalid()
        {
            var command = new UpdateEntryCommand(Guid.NewGuid(), 0, "x", Guid.NewGuid());
            _currentUserMock.Setup(u => u.Roles).Returns(new[] { UserRoles.Manager });

            await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_NotFoundException_When_Entry_Not_Found()
        {
            var command = new UpdateEntryCommand(Guid.NewGuid(), 100, "x", Guid.NewGuid());
            _currentUserMock.Setup(u => u.Roles).Returns(new[] { UserRoles.Manager });

            _entryRepositoryMock.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync((Entry?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_NotFoundException_When_Category_Not_Found()
        {
            var entry = new Entry(100, "old", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var command = new UpdateEntryCommand(entry.Id, 100, "desc", Guid.NewGuid());

            _currentUserMock.Setup(u => u.Roles).Returns(new[] { UserRoles.Manager });
            _entryRepositoryMock.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync(entry);
            _categoryRepositoryMock.Setup(r => r.GetByIdAsync(command.CategoryId)).ReturnsAsync((Category?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}