using CashFlow.IdentityAndAccess.Application.Users.Command;
using CashFlow.IdentityAndAccess.Application.Users.CommandHandler;
using CashFlow.IdentityAndAccess.Application.Users.Dto;
using CashFlow.IdentityAndAccess.Domain.Entities;
using CashFlow.IdentityAndAccess.Domain.Services;
using CashFlow.IdentityAndAccess.Domain.Users.Repositories;
using CashFlow.Shared.Exceptions.Application;
using Microsoft.Extensions.Configuration;
using Moq;


namespace CashFlow.IdentityAndAccess.Tests.Application.Users.CommandHandler
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IAppUserQueryRepository> _userRepository = new();
        private readonly Mock<IHashService> _hashService = new();
        private readonly Mock<IJwtService> _jwtService = new();
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepository = new();
        private readonly Mock<IConfiguration> _configuration = new();

        private LoginCommandHandler CreateHandler()
        {
            return new LoginCommandHandler(
                _userRepository.Object,
                _hashService.Object,
                _jwtService.Object,
                _refreshTokenRepository.Object,
                _configuration.Object
            );
        }

        private static LoginCommand CreateValidCommand()
        {
            return new LoginCommand(new LoginRequestDto
            {
                LoginOrEmail = "testuser",
                Password = "Test@123"
            });
        }

        [Fact]
        public async Task Handle_Should_Login_Successfully()
        {
            // Arrange
            var handler = CreateHandler();
            var command = CreateValidCommand();

            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                LoginName = "testuser",
                PasswordHash = "hashedpass",
                Enabled = true,
                UserRoles = new List<UserRole>()
            };

            _userRepository.Setup(x => x.GetUserWithRolesByLoginOrEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _hashService.Setup(x => x.VerifyHash(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var tokenDto = new JwtTokenDto { Token = "access-token", ExpiresAt = DateTime.UtcNow.AddHours(1) };
            _jwtService.Setup(x => x.GenerateToken(It.IsAny<AppUser>()))
             .Returns(("access-token", DateTime.UtcNow.AddHours(1)));
            _jwtService.Setup(x => x.GenerateRefreshToken()).Returns("refresh-token");
            _configuration.Setup(x => x["Jwt:DaysToExpireRefreshToken"]).Returns("7");

            _refreshTokenRepository.Setup(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("access-token", result.AccessToken);
            Assert.Equal("refresh-token", result.RefreshToken);
        }

        [Theory]
        [InlineData(false, true, true)] // user não existe
        [InlineData(true, false, true)] // senha inválida
        [InlineData(true, true, false)] // usuário desabilitado
        public async Task Handle_Should_Throw_UnauthorizedException_For_Invalid_Credentials(
            bool userExists, bool passwordValid, bool enabled)
        {
            // Arrange
            var handler = CreateHandler();
            var command = CreateValidCommand();

            AppUser? user = userExists
                ? new AppUser { Id = Guid.NewGuid(), LoginName = "testuser", PasswordHash = "hashedpass", Enabled = enabled }
                : null;

            _userRepository.Setup(x => x.GetUserWithRolesByLoginOrEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _hashService.Setup(x => x.VerifyHash(It.IsAny<string>(), It.IsAny<string>())).Returns(passwordValid);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_Throw_ConfigurationException_When_Config_Missing()
        {
            // Arrange
            var handler = CreateHandler();
            var command = CreateValidCommand();

            var user = new AppUser { Id = Guid.NewGuid(), LoginName = "testuser", PasswordHash = "hashedpass", Enabled = true };
            _userRepository.Setup(x => x.GetUserWithRolesByLoginOrEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _hashService.Setup(x => x.VerifyHash(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var tokenDto = new JwtTokenDto { Token = "access-token", ExpiresAt = DateTime.UtcNow.AddHours(1) };
            _jwtService.Setup(x => x.GenerateToken(It.IsAny<AppUser>()))
             .Returns(("access-token", DateTime.UtcNow.AddHours(1)));
            _jwtService.Setup(x => x.GenerateRefreshToken()).Returns("refresh-token");
            _configuration.Setup(x => x["Jwt:DaysToExpireRefreshToken"]).Returns((string?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ConfigurationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_Throw_ConfigurationException_When_Config_Is_Invalid()
        {
            // Arrange
            var handler = CreateHandler();
            var command = CreateValidCommand();

            var user = new AppUser { Id = Guid.NewGuid(), LoginName = "testuser", PasswordHash = "hashedpass", Enabled = true };
            _userRepository.Setup(x => x.GetUserWithRolesByLoginOrEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _hashService.Setup(x => x.VerifyHash(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var tokenDto = new JwtTokenDto { Token = "access-token", ExpiresAt = DateTime.UtcNow.AddHours(1) };
            _jwtService.Setup(x => x.GenerateToken(It.IsAny<AppUser>()))
             .Returns(("access-token", DateTime.UtcNow.AddHours(1)));
            _jwtService.Setup(x => x.GenerateRefreshToken()).Returns("refresh-token");
            _configuration.Setup(x => x["Jwt:DaysToExpireRefreshToken"]).Returns("notAnInt");

            // Act & Assert
            await Assert.ThrowsAsync<ConfigurationException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}