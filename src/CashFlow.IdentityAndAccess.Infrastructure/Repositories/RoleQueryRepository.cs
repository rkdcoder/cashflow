using CashFlow.IdentityAndAccess.Domain.Entities;
using CashFlow.IdentityAndAccess.Domain.Roles.Repositories;
using CashFlow.IdentityAndAccess.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.IdentityAndAccess.Infrastructure.Repositories
{
    public class RoleQueryRepository : IRoleQueryRepository
    {
        private readonly IdentityDbContext _context;

        public RoleQueryRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.AsNoTracking().ToListAsync();
        }
    }
}