using CashFlow.IdentityAndAccess.Application.Users.Command;
using CashFlow.IdentityAndAccess.Application.Users.Dto;
using CashFlow.IdentityAndAccess.Domain.Entities;
using CashFlow.IdentityAndAccess.Domain.Services;
using CashFlow.IdentityAndAccess.Domain.Users.Repositories;
using CashFlow.Shared.Exceptions.Application;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace CashFlow.IdentityAndAccess.Application.Users.CommandHandler
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly IAppUserQueryRepository _userRepository;
        private readonly IHashService _hashService;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;

        public LoginCommandHandler(
            IAppUserQueryRepository userRepository,
            IHashService hashService,
            IJwtService jwtService,
            IRefreshTokenRepository refreshTokenRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _hashService = hashService;
            _jwtService = jwtService;
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserWithRolesByLoginOrEmailAsync(request.Login.LoginOrEmail.Trim());

            if (user == null || !_hashService.VerifyHash(request.Login.Password, user.PasswordHash) || !user.Enabled)
                throw new UnauthorizedException("Invalid credentials or user disabled.");

            // Generate tokens
            var accessToken = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var daysToExpireRefreshToken = _configuration["Jwt:DaysToExpireRefreshToken"];
            if (string.IsNullOrEmpty(daysToExpireRefreshToken))
                throw new ConfigurationException("JWT DaysToExpireRefreshToken is not configured.");

            if (!int.TryParse(daysToExpireRefreshToken, out var days))
                throw new ConfigurationException("JWT DaysToExpireRefreshToken must be a valid integer.");

            var refreshEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                Expiry = DateTime.UtcNow.AddDays(Convert.ToDouble(daysToExpireRefreshToken)),
                IsRevoked = false,
            };
            await _refreshTokenRepository.AddAsync(refreshEntity, cancellationToken);

            return new LoginResponseDto
            {
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken,
                ExpiresAt = accessToken.ExpiresAt
            };
        }
    }
}