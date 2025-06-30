using CashFlow.IdentityAndAccess.Domain.Entities;
using CashFlow.IdentityAndAccess.Domain.Users.Repositories;
using CashFlow.IdentityAndAccess.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.IdentityAndAccess.Infrastructure.Repositories
{
    public class AppUserQueryRepository : IAppUserQueryRepository
    {
        private readonly IdentityDbContext _context;

        public AppUserQueryRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<AppUser?> GetUserWithRolesByLoginOrEmailAsync(string loginOrEmail)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    EF.Functions.ILike(u.LoginName, loginOrEmail) ||
                    EF.Functions.ILike(u.Email, loginOrEmail)
                );
        }

        public async Task<AppUser?> GetUserWithRolesByIdAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<List<AppUser>> GetUsersWithRolesAsync(Guid? userId, string? loginOrEmail, bool? enabled)
        {
            var query = _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsNoTracking()
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(u => u.Id == userId.Value);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(loginOrEmail))
                {
                    query = query.Where(u =>
                        EF.Functions.ILike(u.LoginName, $"%{loginOrEmail}%") ||
                        EF.Functions.ILike(u.Email, $"%{loginOrEmail}%"));
                }
                if (enabled.HasValue)
                {
                    query = query.Where(u => u.Enabled == enabled.Value);
                }
            }

            return await query.ToListAsync();
        }
    }
}