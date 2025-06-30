using AutoMapper;
using CashFlow.IdentityAndAccess.Application.Users.Command;
using CashFlow.IdentityAndAccess.Application.Users.CommandHandler;
using CashFlow.IdentityAndAccess.Application.Users.Dto;
using CashFlow.IdentityAndAccess.Application.Users.Validator.Roles;
using CashFlow.IdentityAndAccess.Domain.Entities;
using CashFlow.IdentityAndAccess.Domain.Users.Repositories;
using CashFlow.Shared.Abstractions;
using CashFlow.Shared.Dtos;
using CashFlow.Shared.Exceptions.Application;
using CashFlow.Shared.Utilities;
using Moq;

namespace CashFlow.IdentityAndAccess.Tests.Application.Users.CommandHandler
{
    public class UpdateUserCommandHandlerTests
    {
        private readonly Mock<IAppUserCommandRepository> _userRepository = new();
        private readonly Mock<IRoleRepository> _roleRepository = new();
        private readonly Mock<ITextSanitizerService> _sanitizer = new();
        private readonly Mock<ICurrentUserContext> _currentUser = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<IInputValidatorService> _validator = new();
        private readonly Mock<IRoleExistenceValidatorService> _roleExistenceValidator = new();
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepository = new();

        private UpdateUserCommandHandler CreateHandler()
        {
            return new UpdateUserCommandHandler(
                _userRepository.Object,
                _roleRepository.Object,
                _sanitizer.Object,
                _currentUser.Object,
                _mapper.Object,
                _validator.Object,
                _roleExistenceValidator.Object,
                _refreshTokenRepository.Object
            );
        }
        private static UpdateUserCommand CreateValidCommand(string loginOrEmail = "testuser", bool enabled = true)
        {
            return new UpdateUserCommand(
                new UpdateUserRequestDto
                {
                    LoginOrEmail = loginOrEmail,
                    Roles = new List<string> { "User" },
                    Enabled = enabled
                });
        }

        [Fact]
        public async Task Handle_Should_Throw_ForbiddenException_When_User_Not_Admin()
        {
            // Arrange
            var handler = CreateHandler();
            var command = CreateValidCommand();

            _currentUser.Setup(x => x.Roles).Returns(new List<string> { "User" });

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_Throw_ForbiddenException_When_Updating_Own_User()
        {
            // Arrange
            var handler = CreateHandler();

            var myUserId = Guid.NewGuid();
            var command = CreateValidCommand();

            var user = new AppUser { Id = myUserId, LoginName = "testuser", Email = "test@cashflow.com", Enabled = true, UserRoles = new List<UserRole>() };

            _currentUser.Setup(x => x.Roles).Returns(new List<string> { "Admin" });
            _currentUser.Setup(x => x.Subject).Returns(myUserId.ToString());

            _sanitizer.Setup(x => x.Normalize(It.IsAny<string>())).Returns((string s) => s);
            _validator.Setup(x => x.ValidateLoginName(It.IsAny<string>())).Returns(new ValidationResultDto { Success = true });
            _validator.Setup(x => x.ValidateEmail(It.IsAny<string>())).Returns(new ValidationResultDto { Success = true });

            _userRepository.Setup(x => x.GetByLoginOrEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}