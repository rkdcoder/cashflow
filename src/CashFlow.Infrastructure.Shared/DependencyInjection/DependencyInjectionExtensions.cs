using CashFlow.BuildingBlocks.Domain.Caching;
using CashFlow.Infrastructure.Shared.Caching;
using CashFlow.Infrastructure.Shared.Cors;
using CashFlow.Infrastructure.Shared.Data;
using CashFlow.Infrastructure.Shared.Identity;
using CashFlow.Shared.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Reflection;

namespace CashFlow.Infrastructure.Shared.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddSharedInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
            });


            var redisConnStr = configuration.GetConnectionString("Redis") ?? "localhost:6379";

            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisConnStr));
            services.AddScoped<IRedisCacheService, RedisCacheService>();

            services.AddSingleton<IDatabaseHelper, DatabaseHelper>();
            services.AddCustomCors(configuration);
            services.AddScoped<ICurrentUserContext, CurrentUserContext>();

            return services;
        }
    }
}