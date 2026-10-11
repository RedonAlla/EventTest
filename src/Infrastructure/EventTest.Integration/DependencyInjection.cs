using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Integrations.DelegatingHandlers;
using RA.Utilities.Integrations.Extensions;

namespace EventTest.Integration;

/// <summary>
/// Provides extension methods for registering integration services in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds integration services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The application's <see cref="IConfiguration"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> to allow for chaining of service registrations.</returns>
    public static IServiceCollection AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScopedHttpMessageHandler<RequestResponseLoggingHandler>();
        return services;
    }
}
