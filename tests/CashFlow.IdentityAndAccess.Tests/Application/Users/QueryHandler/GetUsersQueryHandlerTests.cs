using AutoMapper;
using CashFlow.IdentityAndAccess.Application.Users.Dto;
using CashFlow.IdentityAndAccess.Application.Users.Query;
using CashFlow.IdentityAndAccess.Application.Users.QueryHandler;
using CashFlow.IdentityAndAccess.Domain.Entities;
using CashFlow.IdentityAndAccess.Domain.Users.Repositories;
using Moq;

namespace CashFlow.IdentityAndAccess.Tests.Application.Users.QueryHandler
{
    public class GetUsersQueryHandlerTests
    {
        private readonly Mock<IAppUserQueryRepository> _userQueryRepository = new();
        private readonly Mock<IMapper> _mapper = new();

        private GetUsersQueryHandler CreateHandler()
        {
            return new GetUsersQueryHandler(_userQueryRepository.Object, _mapper.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Mapped_User_List()
        {
            // Arrange
            var handler = CreateHandler();
            var userId = System.Guid.NewGuid();
            var request = new GetUsersQuery(userId, "login", true);

            var domainUsers = new List<AppUser>
            {
                new AppUser { Id = System.Guid.NewGuid(), LoginName = "user1", Email = "u1@test.com", Enabled = true },
                new AppUser { Id = System.Guid.NewGuid(), LoginName = "user2", Email = "u2@test.com", Enabled = true }
            };

            var userListDtos = new List<UserListItemDto>
            {
                new UserListItemDto { LoginName = "user1", Email = "u1@test.com" },
                new UserListItemDto { LoginName = "user2", Email = "u2@test.com" }
            };

            _userQueryRepository.Setup(x => x.GetUsersWithRolesAsync(userId, "login", true))
                .ReturnsAsync(domainUsers);

            _mapper.Setup(x => x.Map<List<UserListItemDto>>(domainUsers))
                .Returns(userListDtos);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("user1", result[0].LoginName);
            Assert.Equal("user2", result[1].LoginName);
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_List_When_No_Users()
        {
            // Arrange
            var handler = CreateHandler();
            var userId = System.Guid.NewGuid();
            var request = new GetUsersQuery(userId, null, null);

            _userQueryRepository.Setup(x => x.GetUsersWithRolesAsync(userId, null, null))
                .ReturnsAsync(new List<AppUser>());

            _mapper.Setup(x => x.Map<List<UserListItemDto>>(It.IsAny<List<AppUser>>()))
                .Returns(new List<UserListItemDto>());

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}