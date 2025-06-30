using AutoMapper;
using CashFlow.Entries.Application.Categories.Dto;
using CashFlow.Entries.Application.Categories.Query;
using CashFlow.Entries.Application.Categories.QueryHandler;
using CashFlow.Entries.Domain.Categories.Repositories;
using CashFlow.Entries.Domain.Entries.Entities;
using Moq;

namespace CashFlow.Entries.Tests.Application.Categories.QueryHandler
{
    public class GetAllCategoriesQueryHandlerTests
    {
        private readonly Mock<ICategoryRepository> _repositoryMock = new();
        private readonly Mock<IMapper> _mapperMock = new();

        private readonly GetAllCategoriesQueryHandler _handler;

        public GetAllCategoriesQueryHandlerTests()
        {
            _handler = new GetAllCategoriesQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Should_Return_List_Of_CategoryDto()
        {
            // Arrange
            var domainCategories = new List<Category>
            {
                new Category(Guid.NewGuid(), "Food", Guid.NewGuid()),
                new Category(Guid.NewGuid(), "Transport", Guid.NewGuid())
            };

            var expectedDtos = new List<CategoryDto>
            {
                new CategoryDto { Name = "Food" },
                new CategoryDto { Name = "Transport" }
            };

            _repositoryMock.Setup(r => r.GetAllWithEntryTypeAsync())
                .ReturnsAsync(domainCategories);

            _mapperMock.Setup(m => m.Map<List<CategoryDto>>(domainCategories))
                .Returns(expectedDtos);

            // Act
            var result = await _handler.Handle(new GetAllCategoriesQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Food", result[0].Name);
            Assert.Equal("Transport", result[1].Name);
        }
    }
}