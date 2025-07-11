using CashFlow.Consolidations.Api.Extensions;
using CashFlow.Infrastructure.Shared.ApiVersioning;
using CashFlow.Infrastructure.Shared.Authentication;
using CashFlow.Infrastructure.Shared.DependencyInjection;
using CashFlow.Infrastructure.Shared.Swagger;
using CashFlow.ServiceDefaults;
using CashFlow.Consolidations.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddDefaultLogging();

builder.AddServiceDefaults();
builder.Services.AddSharedInfrastructureServices(builder.Configuration)
    .AddConsolidationsApplication()
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


app.Run();
