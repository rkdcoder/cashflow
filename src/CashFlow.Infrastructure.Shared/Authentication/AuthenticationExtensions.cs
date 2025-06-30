using CashFlow.Shared.Exceptions.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CashFlow.Infrastructure.Shared.Authentication
{
    /// <summary>
    /// Extension methods for configuring JWT authentication scheme.
    /// </summary>
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Configures JWT authentication with environment-aware settings.
        /// </summary>
        /// <param name="services">Service collection to configure.</param>
        /// <param name="env">Web host environment.</param>
        /// <param name="configuration">Application configuration.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddCustomAuthentication(
            this IServiceCollection services,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("Jwt");

            var secret = jwtSection.GetValue<string>("Secret");
            if (string.IsNullOrWhiteSpace(secret))
                throw new ConfigurationException("JWT Secret not configured.");

            var validateIssuer = jwtSection.GetValue<bool>("ValidateIssuer");
            var issuer = jwtSection.GetValue<string>("Issuer");
            var validateAudience = jwtSection.GetValue<bool>("ValidateAudience");
            var audience = jwtSection.GetValue<string>("Audience");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = env.IsProduction();
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                        ValidateIssuer = validateIssuer,
                        ValidIssuer = issuer,
                        ValidateAudience = validateAudience,
                        ValidAudience = audience,
                        ClockSkew = TimeSpan.Zero,
                        ValidateLifetime = true,
                        NameClaimType = System.Security.Claims.ClaimTypes.Name,
                        RoleClaimType = System.Security.Claims.ClaimTypes.Role
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            var response = new
                            {
                                success = false,
                                message = "Unauthorized: Access token is missing or invalid.",
                                data = null as string
                            };
                            var json = System.Text.Json.JsonSerializer.Serialize(response);
                            return context.Response.WriteAsync(json);
                        }
                    };
                });

            return services;
        }
    }
}
