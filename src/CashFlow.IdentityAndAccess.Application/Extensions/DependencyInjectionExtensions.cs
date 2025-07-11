﻿using CashFlow.Shared.Serialization;
using CashFlow.Shared.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.IdentityAndAccess.Application.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddIdentityAndAccessApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSingleton<ITextSanitizerService, TextSanitizerService>();
            services.AddSingleton<IInputValidatorService, InputValidatorService>();
            services.AddSingleton<IJsonSerializerService, JsonSerializerService>();

            return services;
        }
    }
}