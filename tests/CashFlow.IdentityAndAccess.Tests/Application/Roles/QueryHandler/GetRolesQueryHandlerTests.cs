using AutoMapper;
using CashFlow.IdentityAndAccess.Application.Roles.Dto;
using CashFlow.IdentityAndAccess.Application.Roles.Query;
using CashFlow.IdentityAndAccess.Application.Roles.QueryHandler;
using CashFlow.IdentityAndAccess.Domain.Entities;
using CashFlow.IdentityAndAccess.Domain.Roles.Repositories;
using Moq;

namespace CashFlow.IdentityAndAccess.Tests.Application.Roles.QueryHandler
{
    public class GetRolesQueryHandlerTests
    {
        private readonly Mock<IRoleQueryRepository> _repositoryMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly GetRolesQueryHandler _handler;

        public GetRolesQueryHandlerTests()
        {
            _handler = new GetRolesQueryHandler(
                _repositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Should_Return_SimpleRoleDto_List_When_Roles_Exist()
        {
            // Arrange
            var roles = new List<Role>
            {
                new Role { Id = Guid.NewGuid(), Name = "ADMIN" },
                new Role { Id = Guid.NewGuid(), Name = "MANAGER" }
            };

            var expected = new List<SimpleRoleDto>
            {
                new SimpleRoleDto { Name = "ADMIN" },
                new SimpleRoleDto { Name = "MANAGER" }
            };

            _repositoryMock.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(roles);
            _mapperMock.Setup(m => m.Map<List<SimpleRoleDto>>(roles)).Returns(expected);

            var query = new GetRolesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("ADMIN", result[0].Name);
            Assert.Equal("MANAGER", result[1].Name);
        }

        [Fact]
        public async Task Should_Return_Empty_List_When_No_Roles_Exist()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(new List<Role>());
            _mapperMock.Setup(m => m.Map<List<SimpleRoleDto>>(It.IsAny<List<Role>>()))
                .Returns(new List<SimpleRoleDto>());

            var query = new GetRolesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}