using CashFlow.IdentityAndAccess.Domain.Entities;
using CashFlow.IdentityAndAccess.Domain.Users.Repositories;
using CashFlow.IdentityAndAccess.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.IdentityAndAccess.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for Role entity.
    /// </summary>
    public class RoleRepository : IRoleRepository
    {
        private readonly IdentityDbContext _context;

        public RoleRepository(IdentityDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves roles by their names.
        /// </summary>
        /// <param name="roleNames">Collection of role names (must be normalized).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Read-only list of Role entities.</returns>
        public async Task<IReadOnlyList<Role>> GetByNamesAsync(IEnumerable<string> roleNames, CancellationToken cancellationToken)
        {
            var names = roleNames.ToList();

            return await _context.Roles
                .Where(r => names.Any(name => EF.Functions.ILike(r.Name, name)))
                .ToListAsync(cancellationToken);
        }
    }
}