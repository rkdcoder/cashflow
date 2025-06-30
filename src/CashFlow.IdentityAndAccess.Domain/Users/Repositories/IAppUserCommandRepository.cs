using CashFlow.IdentityAndAccess.Domain.Entities;

namespace CashFlow.IdentityAndAccess.Domain.Users.Repositories
{
    public interface IAppUserCommandRepository
    {
        Task<bool> ExistsByLoginOrEmailAsync(string loginName, string email);
        Task AddAsync(AppUser user, CancellationToken cancellationToken);
        Task<AppUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<AppUser?> GetByLoginOrEmailAsync(string loginOrEmail, CancellationToken cancellationToken);
        Task UpdateAsync(AppUser user, CancellationToken cancellationToken);
    }
}