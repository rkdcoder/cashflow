using CashFlow.IdentityAndAccess.Application.Users.Command;
using CashFlow.IdentityAndAccess.Application.Users.CommandHandler;
using CashFlow.IdentityAndAccess.Domain.Entities;
using CashFlow.IdentityAndAccess.Domain.Services;
using CashFlow.IdentityAndAccess.Domain.Users.Repositories;
using CashFlow.Shared.Abstractions;
using CashFlow.Shared.Dtos;
using CashFlow.Shared.Exceptions.Application;
using CashFlow.Shared.Utilities;
using Moq;

namespace CashFlow.IdentityAndAccess.Tests.Application.Users.CommandHandler
{
    public class ChangeUserPasswordCommandHandlerTests
    {
        private readonly Mock<IAppUserCommandRepository> _userRepoMock = new();
        private readonly Mock<IRefreshTokenRepository> _tokenRepoMock = new();
        private readonly Mock<IHashService> _hashMock = new();
        private readonly Mock<ITextSanitizerService> _sanitizerMock = new();
        private readonly Mock<IInputValidatorService> _validatorMock = new();
        private readonly Mock<ICurrentUserContext> _currentUserMock = new();

        private readonly ChangeUserPasswordCommandHandler _handler;

        public ChangeUserPasswordCommandHandlerTests()
        {
            _handler = new ChangeUserPasswordCommandHandler(
                _userRepoMock.Object,
                _currentUserMock.Object,
                _hashMock.Object,
                _sanitizerMock.Object,
                _validatorMock.Object,
                _tokenRepoMock.Object
            );
        }

        [Fact]
        public async Task Should_Change_Password_Successfully_For_Self()
        {
            var userId = Guid.NewGuid();
            var newPassword = "NewSecure123!";
            var command = new ChangeUserPasswordCommand(new()
            {
                LoginOrEmail = "user@email.com",
                NewPassword = newPassword
            });

            _sanitizerMock.Setup(x => x.Normalize(It.IsAny<string>())).Returns(command.Request.LoginOrEmail);
            _validatorMock.Setup(x => x.ValidateLoginName(It.IsAny<string>())).Returns(ValidationResultDto.SuccessResult());
            _validatorMock.Setup(x => x.ValidateEmail(It.IsAny<string>())).Returns(ValidationResultDto.SuccessResult());
            _validatorMock.Setup(x => x.ValidatePassword(newPassword)).Returns(ValidationResultDto.SuccessResult());
            _userRepoMock.Setup(x => x.GetByLoginOrEmailAsync(command.Request.LoginOrEmail, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new AppUser { Id = userId, UserRoles = [] });
            _currentUserMock.Setup(x => x.Subject).Returns(userId.ToString());
            _currentUserMock.Setup(x => x.Roles).Returns(["Manager"]);
            _hashMock.Setup(x => x.HashData(newPassword)).Returns("hashed");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal("Password changed successfully.", result.Message);
            _userRepoMock.Verify(x => x.UpdateAsync(It.Is<AppUser>(u => u.PasswordHash == "hashed"), It.IsAny<CancellationToken>()), Times.Once);
            _tokenRepoMock.Verify(x => x.RevokeAllByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_BadRequest_When_Login_Is_Empty()
        {
            var command = new ChangeUserPasswordCommand(new()
            {
                LoginOrEmail = " ",
                NewPassword = "abc"
            });

            _sanitizerMock.Setup(x => x.Normalize(It.IsAny<string>())).Returns("");

            await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_NotFound_When_User_Not_Exist()
        {
            var command = new ChangeUserPasswordCommand(new()
            {
                LoginOrEmail = "nonexistent@email.com",
                NewPassword = "SomePassword123"
            });

            _sanitizerMock.Setup(x => x.Normalize(It.IsAny<string>())).Returns(command.Request.LoginOrEmail);
            _validatorMock.Setup(x => x.ValidateLoginName(It.IsAny<string>())).Returns(ValidationResultDto.FailureResult("Invalid name"));
            _validatorMock.Setup(x => x.ValidateEmail(It.IsAny<string>())).Returns(ValidationResultDto.SuccessResult());
            _validatorMock.Setup(x => x.ValidatePassword(command.Request.NewPassword)).Returns(ValidationResultDto.SuccessResult());
            _userRepoMock.Setup(x => x.GetByLoginOrEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((AppUser?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}