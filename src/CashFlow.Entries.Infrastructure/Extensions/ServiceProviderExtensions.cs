using CashFlow.Entries.Infrastructure.Data;
using CashFlow.Shared.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Entries.Infrastructure.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static async Task MigrateAndSeedEntriesAsync(this IServiceProvider services, string connectionString)
        {
            using var scope = services.CreateScope();
            var databaseHelper = scope.ServiceProvider.GetRequiredService<IDatabaseHelper>();
            databaseHelper.EnsureDatabaseExists(connectionString);

            var dbContext = scope.ServiceProvider.GetRequiredService<EntriesDbContext>();

            // Verification for pending migrations
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                Console.WriteLine($"Aplicando migrações: {string.Join(", ", pendingMigrations)}");
                await dbContext.Database.MigrateAsync();
            }
        }
    }
}