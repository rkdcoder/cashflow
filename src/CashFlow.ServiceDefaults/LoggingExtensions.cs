using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;

namespace CashFlow.ServiceDefaults
{
    public static class LoggingExtensions
    {
        public static WebApplicationBuilder AddDefaultLogging(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddOpenTelemetry(options =>
            {
                options.IncludeScopes = true;
                options.ParseStateValues = true;

            });

            return builder;
        }
    }
}
