using CashFlow.Entries.Application.Categories.Command;
using CashFlow.Entries.Application.Categories.CommandHandler;
using CashFlow.Entries.Domain.Categories.Repositories;
using CashFlow.Entries.Domain.Entries.Entities;
using CashFlow.Entries.Domain.Entries.Repositories;
using CashFlow.Shared.Abstractions;
using CashFlow.Shared.Exceptions.Application;
using CashFlow.Shared.Utilities;
using CashFlow.Shared.ValueObjects;
using Moq;
namespace CashFlow.Entries.Tests.Application.Categories.CommandHandler
{
    public class CreateCategoryCommandHandlerTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
        private readonly Mock<IEntryTypeRepository> _entryTypeRepositoryMock = new();
        private readonly Mock<ITextSanitizerService> _sanitizerMock = new();
        private readonly Mock<ICurrentUserContext> _currentUserMock = new();

        private readonly CreateCategoryCommandHandler _handler;

        public CreateCategoryCommandHandlerTests()
        {
            _handler = new CreateCategoryCommandHandler(
                _categoryRepositoryMock.Object,
                _entryTypeRepositoryMock.Object,
                _sanitizerMock.Object,
                _currentUserMock.Object);
        }

        [Fact]
        public async Task Should_Create_Category_When_Valid_Request()
        {
            // Arrange
            var command = new CreateCategoryCommand("Test Category", Guid.NewGuid());
            var sanitizedName = "test category";
            var entryType = new EntryType(command.EntryTypeId, "Revenue");

            _sanitizerMock.Setup(s => s.Normalize(command.Name)).Returns(sanitizedName);
            _categoryRepositoryMock.Setup(r => r.GetByNameAsync(sanitizedName)).ReturnsAsync((Category?)null);
            _entryTypeRepositoryMock.Setup(r => r.GetByIdAsync(command.EntryTypeId)).ReturnsAsync(entryType);
            _currentUserMock.Setup(u => u.Roles).Returns(new[] { UserRoles.Manager });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Category created successfully.", result.Message);
            Assert.NotEqual(Guid.Empty, result.Data);
            _categoryRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_ForbiddenException_When_User_Has_Insufficient_Role()
        {
            // Arrange
            var command = new CreateCategoryCommand("Test", Guid.NewGuid());
            _currentUserMock.Setup(u => u.Roles).Returns(new[] { UserRoles.User });

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_ConflictException_When_Category_Already_Exists()
        {
            // Arrange
            var command = new CreateCategoryCommand("Duplicated", Guid.NewGuid());
            var existingCategory = new Category(Guid.NewGuid(), "duplicated", command.EntryTypeId);

            _sanitizerMock.Setup(s => s.Normalize(command.Name)).Returns("duplicated");
            _categoryRepositoryMock.Setup(r => r.GetByNameAsync("duplicated")).ReturnsAsync(existingCategory);
            _currentUserMock.Setup(u => u.Roles).Returns(new[] { UserRoles.Manager });

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_BadRequestException_When_EntryType_Not_Found()
        {
            // Arrange
            var command = new CreateCategoryCommand("Test", Guid.NewGuid());
            _sanitizerMock.Setup(s => s.Normalize(command.Name)).Returns("test");
            _categoryRepositoryMock.Setup(r => r.GetByNameAsync("test")).ReturnsAsync((Category?)null);
            _entryTypeRepositoryMock.Setup(r => r.GetByIdAsync(command.EntryTypeId)).ReturnsAsync((EntryType?)null);
            _currentUserMock.Setup(u => u.Roles).Returns(new[] { UserRoles.Manager });

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
