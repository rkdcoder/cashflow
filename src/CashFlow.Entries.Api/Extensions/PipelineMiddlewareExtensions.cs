using CashFlow.Infrastructure.Shared.Exceptions;
using CashFlow.Infrastructure.Shared.Logging;
using CashFlow.Infrastructure.Shared.Swagger;

namespace CashFlow.Entries.Api.Extensions
{
    /// <summary>
    /// Registers the full set of standard API middlewares.
    /// Use this as a single entry point for all essential pipeline components.
    /// </summary>
    public static class PipelineMiddlewareExtensions
    {
        /// <summary>
        /// Adds all critical middleware components to the application pipeline:
        /// - Swagger Basic authentication
        /// - Global exception handling
        /// - Exception logging
        /// - Request buffering
        /// - HTTP request logging (database)
        /// </summary>
        public static IApplicationBuilder UseApiMiddlewares(this IApplicationBuilder app)
        {

            // Log HTTP requests and responses to database (background, non-blocking)
            app.UseMiddleware<HttpRequestLoggingMiddleware>();

            // standardized error middleware
            app.UseMiddleware<ApiExceptionMiddleware>();

            return app;
        }
    }
}