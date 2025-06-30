using CashFlow.IdentityAndAccess.Application.Users.Validator.Roles;
using CashFlow.IdentityAndAccess.Domain.Roles.Repositories;
using CashFlow.IdentityAndAccess.Domain.Services;
using CashFlow.IdentityAndAccess.Domain.Users.Repositories;
using CashFlow.IdentityAndAccess.Infrastructure.Authentication;
using CashFlow.IdentityAndAccess.Infrastructure.Data;
using CashFlow.IdentityAndAccess.Infrastructure.Repositories;
using CashFlow.IdentityAndAccess.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.IdentityAndAccess.Infrastructure.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddIdentityAndAccessInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("IdentityDb"))
            );

            services.AddScoped<IHashService, BCryptHashService>();
            services.AddScoped<IAppUserQueryRepository, AppUserQueryRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IJwtService, JwtService>();          
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IAppUserCommandRepository, AppUserCommandRepository>();
            services.AddScoped<IRoleExistenceValidatorService, RoleExistenceValidatorService>();
            services.AddScoped<IRoleQueryRepository, RoleQueryRepository>();

            return services;
        }
    }
}
