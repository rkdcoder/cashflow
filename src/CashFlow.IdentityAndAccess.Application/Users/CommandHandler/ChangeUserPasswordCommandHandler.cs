using CashFlow.IdentityAndAccess.Application.Users.Command;
using CashFlow.IdentityAndAccess.Domain.Services;
using CashFlow.IdentityAndAccess.Domain.Users.Repositories;
using CashFlow.Shared.Abstractions;
using CashFlow.Shared.Dtos;
using CashFlow.Shared.Exceptions.Application;
using CashFlow.Shared.Utilities;
using MediatR;
using CashFlow.Shared.ValueObjects;

namespace CashFlow.IdentityAndAccess.Application.Users.CommandHandler
{
    public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, CommandResultDto<object>>
    {
        private readonly IAppUserCommandRepository _userRepository;
        private readonly ICurrentUserContext _currentUser;
        private readonly IHashService _hashService;
        private readonly ITextSanitizerService _sanitizer;
        private readonly IInputValidatorService _validator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public ChangeUserPasswordCommandHandler(
            IAppUserCommandRepository userRepository,
            ICurrentUserContext currentUser,
            IHashService hashService,
            ITextSanitizerService sanitizer,
            IInputValidatorService validator,
            IRefreshTokenRepository refreshTokenRepository
            )
        {
            _userRepository = userRepository;
            _currentUser = currentUser;
            _hashService = hashService;
            _sanitizer = sanitizer;
            _validator = validator;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<CommandResultDto<object>> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var loginOrEmail = _sanitizer.Normalize(request.Request.LoginOrEmail);

            if (string.IsNullOrWhiteSpace(loginOrEmail))
                throw new BadRequestException("Login or Email is required.");

            var loginValidation = _validator.ValidateLoginName(loginOrEmail);
            var emailValidation = _validator.ValidateEmail(loginOrEmail);
            if (!loginValidation.Success && !emailValidation.Success)
                throw new BadRequestException("Invalid login or email.");

            var passwordValidation = _validator.ValidatePassword(request.Request.NewPassword);
            if (!passwordValidation.Success)
                throw new BadRequestException(passwordValidation.Message ?? "Invalid password.");

            var user = await _userRepository.GetByLoginOrEmailAsync(loginOrEmail, cancellationToken);
            if (user == null)
                throw new NotFoundException("User not found.");

            var currentUserLevel = UserHierarchyUtils.GetHighestRoleLevel(_currentUser.Roles);
            var targetUserLevel = UserHierarchyUtils.GetHighestRoleLevel(user.UserRoles.Select(ur => ur.Role.Name));

            Guid.TryParse(_currentUser.Subject, out var currentUserId);
            var isSelf = user.Id == currentUserId;

            if (!isSelf)
            {
                var requiredRole = UserRoles.Manager;
                if ((currentUserLevel < targetUserLevel) || !UserHierarchyUtils.HasRequiredRole(_currentUser.Roles, requiredRole))
                    throw new ForbiddenException("You do not have permission to change this user's password. You can only change your own password or that of your subordinates.");
            }

            user.PasswordHash = _hashService.HashData(request.Request.NewPassword);

            await _userRepository.UpdateAsync(user, cancellationToken);

            await _refreshTokenRepository.RevokeAllByUserIdAsync(user.Id, cancellationToken);

            return CommandResultDto<object>.SuccessResult("Password changed successfully.");
        }
    }
}
