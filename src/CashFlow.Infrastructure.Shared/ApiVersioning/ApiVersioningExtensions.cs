using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Infrastructure.Shared.ApiVersioning
{
    /// <summary>
    /// Provides extension methods for configuring API versioning.
    /// </summary>
    public static class ApiVersioningExtensions
    {
        /// <summary>
        /// Configures API versioning with URL segment strategy and default v1.0.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <returns>The configured service collection.</returns>
        public static IServiceCollection AddCustomApiVersioning(this IServiceCollection services)
        {
            services
                .AddApiVersioning(options =>
                {
                    // Set default version to 1.0
                    options.DefaultApiVersion = new ApiVersion(1, 0);

                    // Apply default version when not specified
                    options.AssumeDefaultVersionWhenUnspecified = true;

                    // Include version information in responses
                    options.ReportApiVersions = true;

                    // Read version from URL path segment
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                })
                .AddApiExplorer(options =>
                {
                    // Format version group names as 'v{major}{minor}'
                    options.GroupNameFormat = "'v'VVV";

                    // Auto-substitute version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });

            return services;
        }
    }
}