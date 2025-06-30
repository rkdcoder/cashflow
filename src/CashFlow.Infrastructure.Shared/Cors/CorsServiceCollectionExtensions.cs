using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Infrastructure.Shared.Cors
{
    public static class CorsServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
        {
            var corsSettings = new CorsSettings();
            configuration.GetSection("Cors").Bind(corsSettings);

            services.AddCors(options =>
            {
                options.AddPolicy("Default", builder =>
                {
                    builder
                        .WithOrigins(corsSettings.AllowedOrigins ?? Array.Empty<string>())
                        .SetIsOriginAllowedToAllowWildcardSubdomains();

                    if (corsSettings.AllowAnyHeader)
                        builder.AllowAnyHeader();
                    if (corsSettings.AllowAnyMethod)
                        builder.AllowAnyMethod();
                    builder.AllowCredentials();
                });
            });

            return services;
        }
    }
}