using CashFlow.Entries.Domain.Categories.Repositories;
using CashFlow.Entries.Domain.Entries.Repositories;
using CashFlow.Entries.Infrastructure.Data;
using CashFlow.Entries.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Entries.Infrastructure.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddEntriesInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            services.AddDbContext<EntriesDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("CashFlowDb"))
            );

            services.AddScoped<IEntryRepository, EntryRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IEntryTypeRepository, EntryTypeRepository>();

            return services;
        }
    }
}