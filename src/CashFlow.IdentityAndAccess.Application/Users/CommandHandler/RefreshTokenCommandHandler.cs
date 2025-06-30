using CashFlow.IdentityAndAccess.Application.Users.Command;
using CashFlow.IdentityAndAccess.Application.Users.Dto;
using CashFlow.IdentityAndAccess.Domain.Entities;
using CashFlow.IdentityAndAccess.Domain.Services;
using CashFlow.IdentityAndAccess.Domain.Users.Repositories;
using CashFlow.Shared.Exceptions.Application;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace CashFlow.IdentityAndAccess.Application.Users.CommandHandler
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponseDto>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAppUserQueryRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public RefreshTokenCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IAppUserQueryRepository userRepository,
            IJwtService jwtService,
            IConfiguration configuration)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _jwtService = jwtService;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshTokenEntity = await _refreshTokenRepository.GetByTokenAsync(request.Request.RefreshToken, cancellationToken);

            if (refreshTokenEntity == null || refreshTokenEntity.IsRevoked || refreshTokenEntity.Expiry < DateTime.UtcNow)
                throw new UnauthorizedException("Invalid or expired refresh token.");

            var user = await _userRepository.GetUserWithRolesByIdAsync(refreshTokenEntity.UserId);
            if (user == null || !user.Enabled)
                throw new UnauthorizedException("User not found or disabled.");

            // Revoke old token and generate new ones
            refreshTokenEntity.IsRevoked = true;
            refreshTokenEntity.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(refreshTokenEntity, cancellationToken);

            var accessToken = _jwtService.GenerateToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            var daysToExpireRefreshToken = _configuration["Jwt:DaysToExpireRefreshToken"];
            if (string.IsNullOrEmpty(daysToExpireRefreshToken))
                throw new ConfigurationException("JWT DaysToExpireRefreshToken is not configured.");

            if (!int.TryParse(daysToExpireRefreshToken, out var days))
                throw new ConfigurationException("JWT DaysToExpireRefreshToken must be a valid integer.");

            var newRefreshEntity = new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                Expiry = DateTime.UtcNow.AddDays(Convert.ToDouble(daysToExpireRefreshToken)),
                IsRevoked = false
            };
            await _refreshTokenRepository.AddAsync(newRefreshEntity, cancellationToken);

            return new LoginResponseDto
            {
                AccessToken = accessToken.Token,
                RefreshToken = newRefreshToken,
                ExpiresAt = accessToken.ExpiresAt
            };
        }
    }
}