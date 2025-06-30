using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting
{
    public static class Extensions
    {
        public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
        {
            builder.ConfigureOpenTelemetry();
            builder.AddDefaultHealthChecks();

            builder.Services.AddServiceDiscovery();
            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                http.AddStandardResilienceHandler();
                http.AddServiceDiscovery();
            });

            return builder;
        }

        public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
        {
            var aspireEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];  // Aspire Dashboard
            var otlpEndpoint = builder.Configuration["Otlp:Endpoint"];                  // Jaeger/Kibana

            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;

                // Aspire Dashboard  logs
                if (!string.IsNullOrWhiteSpace(aspireEndpoint))
                    logging.AddOtlpExporter(options => options.Endpoint = new Uri(aspireEndpoint));

                // Kibana logs (Elasticsearch)
                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    logging.AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint));
            });

            builder.Services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation();

                    // Aspire Dashboard metrics
                    if (!string.IsNullOrWhiteSpace(aspireEndpoint))
                        metrics.AddOtlpExporter(options => options.Endpoint = new Uri(aspireEndpoint));
                    // Jaeger/Kibana metrics
                    if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                        metrics.AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint));
                })
                .WithTracing(tracing =>
                {
                    if (builder.Environment.IsDevelopment())
                        tracing.SetSampler(new AlwaysOnSampler());

                    tracing.AddAspNetCoreInstrumentation()
                           .AddHttpClientInstrumentation();

                    // Aspire Dashboard  traces
                    if (!string.IsNullOrWhiteSpace(aspireEndpoint))
                        tracing.AddOtlpExporter(options => options.Endpoint = new Uri(aspireEndpoint));
                    // Jaeger traces
                    if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                        tracing.AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint));
                });

            return builder;
        }


        public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
        {
            builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);
            return builder;
        }

        public static WebApplication MapDefaultEndpoints(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapHealthChecks("/health");
                app.MapHealthChecks("/alive", new HealthCheckOptions
                {
                    Predicate = r => r.Tags.Contains("live")
                });
            }
            return app;
        }
    }
}