#pragma warning disable S5122

using System;

namespace EventTest.Api.ServiceConfiguration;

/// <summary>
/// Provides extension methods for registering the CORS policies used by the API.
/// </summary>
public static class CorsExtensions
{
    /// <summary>
    /// The configuration key holding the origins that may call the API outside of the Development environment.
    /// </summary>
    private const string allowedOriginsKey = "Cors:AllowedOrigins";

    /// <summary>
    /// The HTTP methods a cross-origin caller may use outside of the Development environment.
    /// </summary>
    private static readonly string[] allowedMethods =
    [
        "GET",
        "POST",
        "PUT",
        "PATCH",
        "DELETE",
        "OPTIONS"
    ];

    /// <summary>
    /// Adds the CORS policies to the service collection.
    /// </summary>
    /// <remarks>
    /// A single default policy is registered and applied to every endpoint by <c>UseCors</c>:
    /// <list type="bullet">
    ///   <item><description>In the Development environment any origin, header and method is allowed, so local clients need no configuration.</description></item>
    ///   <item><description>In every other environment only the origins listed under <c>Cors:AllowedOrigins</c> are allowed, together with the configured headers and methods. Credentials are never allowed, so a cross-origin caller cannot rely on cookies; send a bearer token in the <c>Authorization</c> header instead.</description></item>
    /// </list>
    /// If <c>Cors:AllowedOrigins</c> is missing or empty outside of the Development environment, every cross-origin request is rejected.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The application's <see cref="IConfiguration"/>.</param>
    /// <param name="environment">The application's <see cref="IHostEnvironment"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddCorsPolicies(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        string[] allowedOrigins = configuration.GetSection(allowedOriginsKey).Get<string[]>() ?? [];

        services.AddCors(options => options.AddDefaultPolicy(policy =>
        {
            if (environment.IsDevelopment())
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();

                return;
            }

            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .WithMethods(allowedMethods);
        }));

        return services;
    }
}

