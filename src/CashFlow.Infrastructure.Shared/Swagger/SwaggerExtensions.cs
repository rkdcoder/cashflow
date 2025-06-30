using Asp.Versioning.ApiExplorer;
using CashFlow.Shared.Exceptions.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace CashFlow.Infrastructure.Shared.Swagger
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddCustomSwagger(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Validate Swagger configuration exists
            var swaggerSection = configuration.GetSection("Swagger");
            if (!swaggerSection.Exists())
                throw new ConfigurationException("Missing or incomplete Swagger configuration in appsettings.json");

            var allowBasic = configuration.GetValue<bool>("Authentication:AllowBasic");

            services.AddSwaggerGen(options =>
            {
                // Lowercase route filter
                options.DocumentFilter<RouteLowercaseDocumentFilter>();

                // XML comments
                var basePath = AppContext.BaseDirectory;
                foreach (var xmlFile in Directory.GetFiles(basePath, "*.xml", SearchOption.TopDirectoryOnly))
                {
                    options.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
                }

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                var securityRequirements = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                };

                if (allowBasic)
                {
                    options.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Scheme = "basic",
                        Type = SecuritySchemeType.Http,
                        Description = "Basic Authentication"
                    });

                    securityRequirements.Add(
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "basic"
                            },
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    );
                }

                options.AddSecurityRequirement(securityRequirements);
            });

            services.ConfigureOptions<SwaggerGenConfigureOptions>();

            return services;
        }

        /// <summary>
        /// Configures Swagger UI middleware with versioned endpoints.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <returns>Configured application builder.</returns>
        public static IApplicationBuilder UseCustomSwaggerUi(
            this IApplicationBuilder app,
            IWebHostEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

                app.UseSwagger();

                app.UseSwaggerUI(options =>
                {
                    // Configure versioned endpoints
                    foreach (var desc in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{desc.GroupName}/swagger.json",
                            $"{desc.ApiVersion}"
                        );
                    }
                    //options.DefaultModelsExpandDepth(-1); // Hide schemas
                    options.EnableFilter();
                });            
            }
            return app;
        }
    }
}