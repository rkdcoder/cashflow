using AutoMapper;
using CashFlow.IdentityAndAccess.Application.Users.Command;
using CashFlow.IdentityAndAccess.Application.Users.Dto;
using CashFlow.IdentityAndAccess.Application.Users.Validator.Roles;
using CashFlow.IdentityAndAccess.Domain.Users.Repositories;
using CashFlow.Shared.Abstractions;
using CashFlow.Shared.Dtos;
using CashFlow.Shared.Exceptions.Application;
using CashFlow.Shared.Utilities;
using CashFlow.Shared.ValueObjects;
using MediatR;

namespace CashFlow.IdentityAndAccess.Application.Users.CommandHandler
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, CommandResultDto<SimpleUserDto>>
    {
        private readonly IAppUserCommandRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ITextSanitizerService _sanitizer;
        private readonly ICurrentUserContext _currentUser;
        private readonly IMapper _mapper;
        private readonly IInputValidatorService _validator;
        private readonly IRoleExistenceValidatorService _roleExistenceValidator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public UpdateUserCommandHandler(
            IAppUserCommandRepository userRepository,
            IRoleRepository roleRepository,
            ITextSanitizerService sanitizer,
            ICurrentUserContext currentUser,
            IMapper mapper,
            IInputValidatorService validator,
            IRoleExistenceValidatorService roleExistenceValidator,
            IRefreshTokenRepository refreshTokenRepository
            )
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _sanitizer = sanitizer;
            _currentUser = currentUser;
            _mapper = mapper;
            _validator = validator;
            _roleExistenceValidator = roleExistenceValidator;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<CommandResultDto<SimpleUserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var requiredRole = UserRoles.Admin;
            if (!UserHierarchyUtils.HasRequiredRole(_currentUser.Roles, requiredRole))
                throw new ForbiddenException($"You do not have sufficient permissions to update users. Required role: {requiredRole}.");

            var loginOrEmail = _sanitizer.Normalize(request.User.LoginOrEmail);

            if (string.IsNullOrWhiteSpace(loginOrEmail))
                throw new BadRequestException("Login or Email is required.");

            var loginValidation = _validator.ValidateLoginName(loginOrEmail);
            var emailValidation = _validator.ValidateEmail(loginOrEmail);
            if (!loginValidation.Success && !emailValidation.Success)
                throw new BadRequestException("Invalid login or email.");

            var user = await _userRepository.GetByLoginOrEmailAsync(loginOrEmail, cancellationToken);
            if (user == null)
                throw new NotFoundException("User not found.");

            Guid.TryParse(_currentUser.Subject, out var currentUserId);
            if (user.Id == currentUserId)
                throw new ForbiddenException("You cannot update your own user infos.");

            var roles = request.User.Roles ?? new List<string>();
            var (rolesExist, notFoundRoles) = await _roleExistenceValidator.ValidateRolesExistAsync(roles, cancellationToken);
            if (!rolesExist)
                throw new NotFoundException($"The following roles do not exist: {string.Join(", ", notFoundRoles ?? new List<string>())}");

            bool wasEnabled = user.Enabled;
            user.Enabled = request.User.Enabled;

            var newRoles = await _roleRepository.GetByNamesAsync(roles, cancellationToken);
            user.UserRoles.Clear();
            foreach (var role in newRoles)
                user.UserRoles.Add(new Domain.Entities.UserRole { User = user, Role = role });

            await _userRepository.UpdateAsync(user, cancellationToken);

            if (wasEnabled && !user.Enabled)
            {
                await _refreshTokenRepository.RevokeAllByUserIdAsync(user.Id, cancellationToken);
            }

            var resultDto = _mapper.Map<SimpleUserDto>(user);
            resultDto.Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            return CommandResultDto<SimpleUserDto>.SuccessResult("User updated successfully.", resultDto);
        }
    }
}