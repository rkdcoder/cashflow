using CashFlow.IdentityAndAccess.Application.Users.Command;
using CashFlow.IdentityAndAccess.Application.Users.CommandHandler;
using CashFlow.IdentityAndAccess.Application.Users.Dto;
using CashFlow.IdentityAndAccess.Domain.Entities;
using CashFlow.IdentityAndAccess.Domain.Services;
using CashFlow.IdentityAndAccess.Domain.Users.Repositories;
using CashFlow.Shared.Exceptions.Application;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.IdentityAndAccess.Tests.Application.Users.CommandHandler
{
    public class RefreshTokenCommandHandlerTests
    {
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepository = new();
        private readonly Mock<IAppUserQueryRepository> _userRepository = new();
        private readonly Mock<IJwtService> _jwtService = new();
        private readonly Mock<IConfiguration> _configuration = new();

        private RefreshTokenCommandHandler CreateHandler()
        {
            return new RefreshTokenCommandHandler(
                _refreshTokenRepository.Object,
                _userRepository.Object,
                _jwtService.Object,
                _configuration.Object
            );
        }

        private static RefreshTokenCommand CreateValidCommand(string token = "valid-refresh-token")
        {
            return new RefreshTokenCommand(
                new RefreshTokenRequestDto { RefreshToken = token }
            );
        }

        [Fact]
        public async Task Handle_Should_RefreshToken_Successfully()
        {
            // Arrange
            var handler = CreateHandler();
            var command = CreateValidCommand();

            var userId = Guid.NewGuid();
            var refreshToken = new RefreshToken
            {
                Token = "valid-refresh-token",
                UserId = userId,
                Expiry = DateTime.UtcNow.AddDays(1),
                IsRevoked = false
            };

            var user = new AppUser
            {
                Id = userId,
                LoginName = "testuser",
                Enabled = true,
                UserRoles = new List<UserRole>()
            };

            _refreshTokenRepository.Setup(x => x.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(refreshToken);

            _userRepository.Setup(x => x.GetUserWithRolesByIdAsync(userId))
                .ReturnsAsync(user);

            _jwtService.Setup(x => x.GenerateToken(user))
                .Returns(("access-token", DateTime.UtcNow.AddHours(1)));

            _jwtService.Setup(x => x.GenerateRefreshToken())
                .Returns("new-refresh-token");

            _configuration.Setup(x => x["Jwt:DaysToExpireRefreshToken"])
                .Returns("7");

            _refreshTokenRepository.Setup(x => x.UpdateAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _refreshTokenRepository.Setup(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("access-token", result.AccessToken);
            Assert.Equal("new-refresh-token", result.RefreshToken);
        }

        [Fact]
        public async Task Handle_Should_Throw_UnauthorizedException_When_Token_Is_Null_Or_Invalid()
        {
            // Arrange
            var handler = CreateHandler();
            var command = CreateValidCommand();

            _refreshTokenRepository.Setup(x => x.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RefreshToken?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Theory]
        [InlineData(true, false, true)]  // IsRevoked = true
        [InlineData(false, true, true)]  // Expiry < DateTime.UtcNow
        public async Task Handle_Should_Throw_UnauthorizedException_When_Token_Revoked_Or_Expired(
            bool isRevoked, bool expired, bool validUser)
        {
            // Arrange
            var handler = CreateHandler();
            var command = CreateValidCommand();

            var refreshToken = new RefreshToken
            {
                Token = "some-token",
                UserId = Guid.NewGuid(),
                Expiry = expired ? DateTime.UtcNow.AddDays(-1) : DateTime.UtcNow.AddDays(1),
                IsRevoked = isRevoked
            };

            _refreshTokenRepository.Setup(x => x.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(refreshToken);

            if (validUser)
            {
                var user = new AppUser { Id = refreshToken.UserId, Enabled = true };
                _userRepository.Setup(x => x.GetUserWithRolesByIdAsync(refreshToken.UserId)).ReturnsAsync(user);
            }

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Theory]
        [InlineData(false, true)] // user nulo
        [InlineData(true, false)] // user desabilitado
        public async Task Handle_Should_Throw_UnauthorizedException_When_User_NotFound_Or_Disabled(bool provideUser, bool enabled)
        {
            // Arrange
            var handler = CreateHandler();
            var command = CreateValidCommand();

            var userId = Guid.NewGuid();
            var refreshToken = new RefreshToken
            {
                Token = "some-token",
                UserId = userId,
                Expiry = DateTime.UtcNow.AddDays(1),
                IsRevoked = false
            };

            _refreshTokenRepository.Setup(x => x.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(refreshToken);

            AppUser? user = provideUser ? new AppUser { Id = userId, Enabled = enabled } : null;
            _userRepository.Setup(x => x.GetUserWithRolesByIdAsync(userId)).ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_Throw_ConfigurationException_When_Config_Missing()
        {
            // Arrange
            var handler = CreateHandler();
            var command = CreateValidCommand();

            var userId = Guid.NewGuid();
            var refreshToken = new RefreshToken
            {
                Token = "some-token",
                UserId = userId,
                Expiry = DateTime.UtcNow.AddDays(1),
                IsRevoked = false
            };

            var user = new AppUser { Id = userId, Enabled = true };

            _refreshTokenRepository.Setup(x => x.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(refreshToken);

            _userRepository.Setup(x => x.GetUserWithRolesByIdAsync(userId)).ReturnsAsync(user);

            _configuration.Setup(x => x["Jwt:DaysToExpireRefreshToken"]).Returns((string?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ConfigurationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_Throw_ConfigurationException_When_Config_Invalid()
        {
            // Arrange
            var handler = CreateHandler();
            var command = CreateValidCommand();

            var userId = Guid.NewGuid();
            var refreshToken = new RefreshToken
            {
                Token = "some-token",
                UserId = userId,
                Expiry = DateTime.UtcNow.AddDays(1),
                IsRevoked = false
            };

            var user = new AppUser { Id = userId, Enabled = true };

            _refreshTokenRepository.Setup(x => x.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(refreshToken);

            _userRepository.Setup(x => x.GetUserWithRolesByIdAsync(userId)).ReturnsAsync(user);

            _configuration.Setup(x => x["Jwt:DaysToExpireRefreshToken"]).Returns("notanumber");

            // Act & Assert
            await Assert.ThrowsAsync<ConfigurationException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
