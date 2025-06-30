using CashFlow.IdentityAndAccess.Domain.Entities;
using CashFlow.IdentityAndAccess.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.IdentityAndAccess.Infrastructure.Data
{
    public static class IdentityDbContextSeed
    {
        public static async Task SeedAdminUserAsync(this IdentityDbContext context, IHashService hashService)
        {
            await SeedUserWithRoleAsync(context, hashService, "ADMIN@OPAH.COM.BR", "ADMIN", "ADMIN");
            await SeedUserWithRoleAsync(context, hashService, "MANAGER@OPAH.COM.BR", "MANAGER", "MANAGER");
            await SeedUserWithRoleAsync(context, hashService, "USER@OPAH.COM.BR", "USER", "USER");
        }

        private static async Task SeedUserWithRoleAsync(
            IdentityDbContext context,
            IHashService hashService,
            string email,
            string login,
            string roleName)
        {
            var passwordHash = hashService.HashData("OpahIt2025");

            var roles = await context.Roles
                .Where(r => r.Name == roleName)
                .ToListAsync();

            if (!roles.Any())
                return;

            var user = await context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.LoginName == login || u.Email == email);

            if (user == null)
            {
                user = new AppUser
                {
                    LoginName = login,
                    Email = email,
                    PasswordHash = passwordHash,
                    Enabled = true,
                };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            var existingRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToHashSet();
            foreach (var role in roles)
            {
                if (!existingRoleIds.Contains(role.Id))
                {
                    user.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = role.Id
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}