using CashFlow.IdentityAndAccess.Domain.Entities;
using CashFlow.IdentityAndAccess.Domain.Users.Repositories;
using CashFlow.IdentityAndAccess.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.IdentityAndAccess.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for write operations of AppUser entity.
    /// </summary>
    public class AppUserCommandRepository : IAppUserCommandRepository
    {
        private readonly IdentityDbContext _context;

        public AppUserCommandRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByLoginOrEmailAsync(string loginName, string email)
        {
            return await _context.Users
                .AnyAsync(u =>
                    EF.Functions.ILike(u.LoginName, loginName) ||
                    EF.Functions.ILike(u.Email, email));
        }

        public async Task AddAsync(AppUser user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<AppUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<AppUser?> GetByLoginOrEmailAsync(string loginOrEmail, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u =>
                    EF.Functions.ILike(u.LoginName, loginOrEmail) ||
                    EF.Functions.ILike(u.Email, loginOrEmail),
                    cancellationToken);
        }

        public async Task UpdateAsync(AppUser user, CancellationToken cancellationToken)
        {
            _context.Entry(user).State = EntityState.Modified;

            foreach (var role in user.UserRoles)
            {
                if (_context.Entry(role).State == EntityState.Detached)
                    _context.Entry(role).State = EntityState.Added;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}