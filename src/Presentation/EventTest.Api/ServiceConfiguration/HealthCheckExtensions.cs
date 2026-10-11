using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EventTest.Api.ServiceConfiguration;

/// <summary>
/// Provides extension methods for registering and mapping default health checks.
/// </summary>
public static class HealthCheckExtensions
{
    /// <summary>
    /// Adds the default health checks to the application builder.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return services;
    }

    /// <summary>
    /// Maps the default health endpoints for readiness and liveness checks.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> to add the middleware to.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> to allow for fluent chaining.</returns>
    public static IApplicationBuilder MapHealthCheckEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks("/alive", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live")
        });

        return app;
    }
}
