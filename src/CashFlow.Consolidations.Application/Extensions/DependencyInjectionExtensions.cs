using CashFlow.Shared.Serialization;
using CashFlow.Shared.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Consolidations.Application.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddConsolidationsApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSingleton<ITextSanitizerService, TextSanitizerService>();
            services.AddSingleton<IInputValidatorService, InputValidatorService>();
            services.AddSingleton<IJsonSerializerService, JsonSerializerService>();

            return services;
        }
    }
}
