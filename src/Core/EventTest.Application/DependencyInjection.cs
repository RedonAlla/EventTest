using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Feature.Extensions;

namespace EventTest.Application;

/// <summary>
/// Provides extension methods for registering application-layer services in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Use this registration only for applications with a single Presentation layer.
    /// </summary>
    /// <remarks>
    /// If multiple Presentation layers exist, register only the services required by each layer.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        _ = services.AddMediator();

        return services;
    }
}
