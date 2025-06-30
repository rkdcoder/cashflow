using CashFlow.IdentityAndAccess.Domain.Entities;

namespace CashFlow.IdentityAndAccess.Domain.Users.Repositories
{
    public interface IAppUserQueryRepository
    {
        Task<AppUser?> GetUserWithRolesByLoginOrEmailAsync(string loginOrEmail);
        Task<AppUser?> GetUserWithRolesByIdAsync(Guid userId);
        Task<List<AppUser>> GetUsersWithRolesAsync(Guid? userId, string? loginOrEmail, bool? enabled);
    }
}