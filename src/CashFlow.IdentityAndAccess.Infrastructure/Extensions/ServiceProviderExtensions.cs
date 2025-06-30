using CashFlow.IdentityAndAccess.Domain.Services;
using CashFlow.IdentityAndAccess.Infrastructure.Data;
using CashFlow.Shared.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.IdentityAndAccess.Infrastructure.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static async Task MigrateAndSeedIdentityAsync(this IServiceProvider services, string connectionString)
        {
            using var scope = services.CreateScope();
            var databaseHelper = scope.ServiceProvider.GetRequiredService<IDatabaseHelper>();
            databaseHelper.EnsureDatabaseExists(connectionString);

            var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

            // Verification for pending migrations
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                Console.WriteLine($"Aplicando migrações: {string.Join(", ", pendingMigrations)}");
                await dbContext.Database.MigrateAsync();
            }

            var hashService = scope.ServiceProvider.GetRequiredService<IHashService>();
            await dbContext.SeedAdminUserAsync(hashService);
        }
    }
}
