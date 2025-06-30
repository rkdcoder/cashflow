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
    public class CreateEntryCommandHandlerTests
    {
        private readonly Mock<IEntryRepository> _entryRepositoryMock = new();
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
        private readonly Mock<ICurrentUserContext> _currentUserMock = new();
        private readonly Mock<IRedisCacheService> _redisCacheMock = new();

        private readonly CreateEntryCommandHandler _handler;

        public CreateEntryCommandHandlerTests()
        {
            _handler = new CreateEntryCommandHandler(
                _entryRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _currentUserMock.Object,
                _redisCacheMock.Object
            );
        }

        [Fact]
        public async Task Should_Create_Entry_When_Valid_Request()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var entryTypeId = Guid.NewGuid();

            var command = new CreateEntryCommand(100, "Test Entry", categoryId);
            var category = new Category(categoryId, "Test", entryTypeId);

            _currentUserMock.Setup(c => c.Subject).Returns(userId.ToString());
            _currentUserMock.Setup(c => c.Roles).Returns(new[] { UserRoles.User });

            _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Entry created successfully.", result.Message);
            Assert.NotEqual(Guid.Empty, result.Data);

            _entryRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Entry>()), Times.Once);
            _redisCacheMock.Verify(c => c.AddOrUpdateAsync(It.IsAny<string>(), It.IsAny<Entry>()), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_ForbiddenException_When_User_Not_Authorized()
        {
            // Arrange
            var command = new CreateEntryCommand(100, "Test Entry", Guid.NewGuid());
            _currentUserMock.Setup(c => c.Roles).Returns(Array.Empty<string>());

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_BadRequestException_When_Amount_Is_Zero()
        {
            // Arrange
            var command = new CreateEntryCommand(0, "Invalid Entry", Guid.NewGuid());
            _currentUserMock.Setup(c => c.Roles).Returns(new[] { UserRoles.User });

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_NotFoundException_When_Category_Not_Exists()
        {
            // Arrange
            var command = new CreateEntryCommand(50, "No Category", Guid.NewGuid());
            _currentUserMock.Setup(c => c.Roles).Returns(new[] { UserRoles.User });

            _categoryRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Category?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}