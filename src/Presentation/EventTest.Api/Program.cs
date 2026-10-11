using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using RA.Utilities.Api.Extensions;
using RA.Utilities.Logging.Core.Extensions;
using EventTest.Api.ServiceConfiguration;
using EventTest.Application;
using EventTest.Infrastructure;
using EventTest.Persistence.Database;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

builder.AddLoggingWithConfiguration();

builder.Services
    .Configure<JsonOptions>(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .AddDefaultHealthChecks()
    .AddOpenApi(configuration)
    .AddCorsPolicies(configuration, builder.Environment)
    .AddRaExceptionHandling()
    .AddProblemDetails()
    .AddApplicationServices()
    .AddInfrastructureServices(configuration)
    .AddDefaultMiddlewares();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await EventTestDbInitializer.InitializeDatabaseAsync(app.Services);
}
if (!app.Environment.IsProduction())
{
    app.UseOpenApi();
}

app.MapHealthCheckEndpoints()
   .UseDefaultHeadersMiddleware()
   .UseLoggingMiddleware()
   .UseCors()
   .UseHttpsRedirection()
   .UseExceptionHandler();

app.MapEndpoints();

await app.RunAsync();
