using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CashFlow.Infrastructure.Shared.Swagger
{
    public class SwaggerGenConfigureOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        private readonly IConfiguration _configuration;

        public SwaggerGenConfigureOptions(
            IApiVersionDescriptionProvider provider,
            IConfiguration configuration)
        {
            _provider = provider;
            _configuration = configuration;
        }

        public void Configure(SwaggerGenOptions options)
        {
            var swaggerSection = _configuration.GetSection("Swagger");
            var title = swaggerSection["Title"];
            var description = swaggerSection["Description"];
            var contactEmail = swaggerSection["ContactEmail"];

            foreach (var desc in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    desc.GroupName,
                    new OpenApiInfo
                    {
                        Title = $"{title} {desc.ApiVersion}",
                        Description = $"{description} {desc.ApiVersion}",
                        Version = desc.GroupName,
                        Contact = new OpenApiContact { Email = contactEmail }
                    });
            }
        }
    }
}