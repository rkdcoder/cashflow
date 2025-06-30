using AutoMapper;
using CashFlow.IdentityAndAccess.Application.Users.Command;
using CashFlow.IdentityAndAccess.Application.Users.CommandHandler;
using CashFlow.IdentityAndAccess.Application.Users.Dto;
using CashFlow.IdentityAndAccess.Application.Users.Validator.Roles;
using CashFlow.IdentityAndAccess.Domain.Entities;
using CashFlow.IdentityAndAccess.Domain.Services;
using CashFlow.IdentityAndAccess.Domain.Users.Repositories;
using CashFlow.Shared.Abstractions;
using CashFlow.Shared.Dtos;
using CashFlow.Shared.Exceptions.Application;
using CashFlow.Shared.Utilities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.IdentityAndAccess.Tests.Application.Users.CommandHandler
{
    public class CreateUserCommandHandlerTests
    {
        private readonly Mock<IAppUserCommandRepository> _userRepository = new();
        private readonly Mock<IRoleRepository> _roleRepository = new();
        private readonly Mock<ITextSanitizerService> _sanitizer = new();
        private readonly Mock<IHashService> _hashService = new();
        private readonly Mock<ICurrentUserContext> _currentUser = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<IInputValidatorService> _validator = new();
        private readonly Mock<IRoleExistenceValidatorService> _roleExistenceValidator = new();

        private CreateUserCommandHandler CreateHandler()
        {
            return new CreateUserCommandHandler(
                _userRepository.Object,
                _roleRepository.Object,
                _sanitizer.Object,
                _hashService.Object,
                _currentUser.Object,
                _mapper.Object,
                _validator.Object,
                _roleExistenceValidator.Object
            );
        }

        private static CreateUserCommand CreateValidCommand()
        {
            return new CreateUserCommand(
                new CreateUserRequestDto
                {
                    LoginName = "testuser",
                    Email = "test@cashflow.com",
                    Password = "ValidPassword@1",
                    Roles = new List<string> { "User" }
                });
        }


        [Fact]
        public async Task Handle_Should_Throw_ForbiddenException_If_User_Not_Admin()
        {
            // Arrange
            var handler = CreateHandler();
            var command = CreateValidCommand();

            _currentUser.Setup(c => c.Roles).Returns(new List<string> { "User" });

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
