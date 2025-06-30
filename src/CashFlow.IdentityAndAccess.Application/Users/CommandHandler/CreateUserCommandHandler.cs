using AutoMapper;
using CashFlow.IdentityAndAccess.Application.Users.Command;
using CashFlow.IdentityAndAccess.Application.Users.Dto;
using CashFlow.IdentityAndAccess.Application.Users.Validator.Roles;
using CashFlow.IdentityAndAccess.Domain.Entities;
using CashFlow.IdentityAndAccess.Domain.Services;
using CashFlow.IdentityAndAccess.Domain.Users.Repositories;
using CashFlow.Shared.Abstractions;
using CashFlow.Shared.Dtos;
using CashFlow.Shared.Exceptions.Application;
using CashFlow.Shared.Utilities;
using CashFlow.Shared.ValueObjects;
using MediatR;

namespace CashFlow.IdentityAndAccess.Application.Users.CommandHandler
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CommandResultDto<SimpleUserDto>>
    {
        private readonly IAppUserCommandRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ITextSanitizerService _sanitizer;
        private readonly IHashService _hashService;
        private readonly ICurrentUserContext _currentUser;
        private readonly IMapper _mapper;
        private readonly IInputValidatorService _validator;
        private readonly IRoleExistenceValidatorService _roleExistenceValidator;

        public CreateUserCommandHandler(
            IAppUserCommandRepository userRepository,
            IRoleRepository roleRepository,
            ITextSanitizerService sanitizer,
            IHashService hashService,
            ICurrentUserContext currentUser,
            IMapper mapper,
            IInputValidatorService validator,
            IRoleExistenceValidatorService roleExistenceValidator)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _sanitizer = sanitizer;
            _hashService = hashService;
            _currentUser = currentUser;
            _mapper = mapper;
            _validator = validator;
            _roleExistenceValidator = roleExistenceValidator;
        }

        public async Task<CommandResultDto<SimpleUserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var requiredRole = UserRoles.Admin;
            if (!UserHierarchyUtils.HasRequiredRole(_currentUser.Roles, requiredRole))
                throw new ForbiddenException($"You do not have sufficient permissions to create users. Required role: {requiredRole}.");

            var userDto = request.User;

            var login = _sanitizer.Normalize(userDto.LoginName);
            var email = _sanitizer.Normalize(userDto.Email);
            var password = userDto.Password;

            var loginValidation = _validator.ValidateLoginName(login);
            if (!loginValidation.Success)
                throw new BadRequestException(loginValidation.Message ?? "Invalid login name.");

            var emailValidation = _validator.ValidateEmail(email);
            if (!emailValidation.Success)
                throw new BadRequestException(emailValidation.Message ?? "Invalid email.");

            var passwordValidation = _validator.ValidatePassword(password);
            if (!passwordValidation.Success)
                throw new BadRequestException(passwordValidation.Message ?? "Invalid password.");

            if (await _userRepository.ExistsByLoginOrEmailAsync(login, email))
                throw new ConflictException("Login or Email already in use.");

            var roles = userDto.Roles ?? new List<string>();
            var (rolesExist, notFoundRoles) = await _roleExistenceValidator.ValidateRolesExistAsync(roles, cancellationToken);
            if (!rolesExist)
                throw new NotFoundException($"The following roles do not exist: {string.Join(", ", notFoundRoles ?? new List<string>())}");

            var user = new AppUser
            {
                LoginName = login,
                Email = email,
                PasswordHash = _hashService.HashData(password),
                Enabled = true,
                UserRoles = new List<UserRole>()
            };

            var availableRoles = await _roleRepository.GetByNamesAsync(roles, cancellationToken);
            foreach (var role in availableRoles)
                user.UserRoles.Add(new UserRole { User = user, Role = role });

            await _userRepository.AddAsync(user, cancellationToken);

            var resultDto = _mapper.Map<SimpleUserDto>(user);
            resultDto.Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            return CommandResultDto<SimpleUserDto>.SuccessResult("User created successfully.", resultDto);
        }
    }
}