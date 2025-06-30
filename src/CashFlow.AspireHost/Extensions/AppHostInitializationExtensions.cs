using CashFlow.AppHost.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.AppHost.Extensions
{
    public static class AppHostInitializationExtensions
    {
        public static IServiceCollection AddAppHostInitializations(this IServiceCollection services)
        {
            services.AddHttpClient("kibana", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5601");
                client.DefaultRequestHeaders.Add("kbn-xsrf", "true");
            });

            services.AddHostedService<KibanaIndexInitializer>();

            return services;
        }
    }
}