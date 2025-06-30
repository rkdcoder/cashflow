using CashFlow.IdentityAndAccess.Domain.Entities;

namespace CashFlow.IdentityAndAccess.Domain.Services
{
    public interface IJwtService
    {
        (string Token, DateTime ExpiresAt) GenerateToken(AppUser user);
        string GenerateRefreshToken();
    }
}