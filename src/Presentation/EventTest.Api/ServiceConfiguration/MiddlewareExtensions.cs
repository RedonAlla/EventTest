using RA.Utilities.Api.Extensions;

namespace EventTest.Api.ServiceConfiguration;

/// <summary>
/// Provides extension methods for registering and using custom middlewares.
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// The maximum length of the request or response body to log in bytes.
    /// </summary>
    private const int maxBodyLogLength = 16384;

    /// <summary>
    /// The response duration threshold in milliseconds above which
    /// the response will be logged at <see cref="Microsoft.Extensions.Logging.LogLevel.Warning"/> instead of
    /// <see cref="Microsoft.Extensions.Logging.LogLevel.Information"/>.
    /// </summary>
    private const int warningThresholdMilliseconds = 16384;

    private static readonly string[] pathsToIgnore =
    [
        "/openapi-ui",
        "/openapi",
        "/health"
    ];

    /// <summary>
    /// Adds custom middlewares to the service collection.
    /// </summary>
    /// <remarks>
    /// This method currently registers the HTTP logging middleware and configures it to ignore
    /// specific paths like the root ("/") for the API documentation UI and the OpenAPI specification endpoint ("/openapi").
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultMiddlewares(this IServiceCollection services)
    {
        return services
            .AddDefaultHeadersMiddleware(options =>
                options.PathsToIgnore.UnionWith(pathsToIgnore))
            .AddLoggingMiddleware(options =>
            {
                options.MaxBodyLogLength = maxBodyLogLength;
                options.WarningThresholdMilliseconds = warningThresholdMilliseconds; //Log Warning if > 2s
                options.PathsToIgnore.UnionWith(pathsToIgnore);
            });
    }
}
