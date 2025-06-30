using CashFlow.Entries.Api.Extensions;
using CashFlow.Entries.Application.Extensions;
using CashFlow.Entries.Infrastructure.DependencyInjection;
using CashFlow.Entries.Infrastructure.Extensions;
using CashFlow.Infrastructure.Shared.ApiVersioning;
using CashFlow.Infrastructure.Shared.Authentication;
using CashFlow.Infrastructure.Shared.DependencyInjection;
using CashFlow.Infrastructure.Shared.Swagger;
using CashFlow.ServiceDefaults;
using CashFlow.BuildingBlocks.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.AddDefaultLogging();

builder.AddServiceDefaults();
builder.Services.AddSharedInfrastructureServices(builder.Configuration)
    .AddEntriesInfrastructureServices(builder.Configuration)
    .AddBuildingBlocksServices()
    .AddEntriesApplication()
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
var connStr = builder.Configuration.GetConnectionString("CashFlowDb")
    ?? throw new InvalidOperationException("Connection string 'CashFlowDb' is not configured.");

await app.Services.MigrateAndSeedEntriesAsync(connStr);
app.Run();