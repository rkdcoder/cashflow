using CashFlow.IdentityAndAccess.Api.Extensions;
using CashFlow.IdentityAndAccess.Infrastructure.DependencyInjection;
using CashFlow.Infrastructure.Shared.ApiVersioning;
using CashFlow.Infrastructure.Shared.Authentication;
using CashFlow.Infrastructure.Shared.DependencyInjection;
using CashFlow.Infrastructure.Shared.Swagger;
using CashFlow.ServiceDefaults;
using CashFlow.IdentityAndAccess.Application.Extensions;
using CashFlow.IdentityAndAccess.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddDefaultLogging();

builder.AddServiceDefaults();
builder.Services.AddSharedInfrastructureServices(builder.Configuration)
    .AddIdentityAndAccessInfrastructureServices(builder.Configuration)
    .AddIdentityAndAccessApplication()
    .AddCustomApiVersioning()
    .AddCustomSwagger(builder.Configuration)
    .AddCustomAuthentication(builder.Environment, builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseCors("Default");
app.UseApiMiddlewares();

app.UseCustomSwaggerUi(builder.Environment);

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// migrations
var connStr = builder.Configuration.GetConnectionString("IdentityDb")
    ?? throw new InvalidOperationException("Connection string 'IdentityDb' is not configured.");

await app.Services.MigrateAndSeedIdentityAsync(connStr);

app.Run();
