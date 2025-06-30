using CashFlow.IdentityAndAccess.Domain.Entities;

namespace CashFlow.IdentityAndAccess.Domain.Users.Repositories
{
    public interface IRoleRepository
    {
        Task<IReadOnlyList<Role>> GetByNamesAsync(IEnumerable<string> roleNames, CancellationToken cancellationToken);
    }
}