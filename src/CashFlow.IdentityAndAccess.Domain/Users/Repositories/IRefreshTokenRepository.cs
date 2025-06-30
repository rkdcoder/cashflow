using CashFlow.IdentityAndAccess.Domain.Entities;

namespace CashFlow.IdentityAndAccess.Domain.Users.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);
        Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        Task RevokeAllByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}