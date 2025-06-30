using CashFlow.IdentityAndAccess.Domain.Entities;

namespace CashFlow.IdentityAndAccess.Domain.Roles.Repositories
{
    public interface IRoleQueryRepository
    {
        Task<List<Role>> GetAllRolesAsync();
    }
}