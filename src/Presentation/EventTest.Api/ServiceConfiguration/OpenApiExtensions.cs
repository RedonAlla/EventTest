using Microsoft.Extensions.Options;
using RA.Utilities.OpenApi.Extensions;
using RA.Utilities.OpenApi.Settings;
using Scalar.AspNetCore;

namespace EventTest.Api.ServiceConfiguration;

/// <summary>
/// Provides extension methods for configuring OpenAPI and the API documentation UI (Scalar or Swagger UI).
/// </summary>
public static class OpenApiExtensions
{
    /// <summary>
    /// The default title for the API documentation UI if not provided in configuration.
    /// </summary>
    private const string Title = "EventTest.Api";

    /// <summary>
    /// The base64 encoded SVG for the Scalar UI favicon, representing the OpenAPI Initiative logo.
    /// </summary>
    private const string FavIcon = "data:image/svg+xml,%3C%3Fxml version='1.0' encoding='utf-8'%3F%3E%3Csvg viewBox='0 0 456.938 360.646' xmlns='http://www.w3.org/2000/svg'%3E%3Cpath style='fill:%23BD0A0A;' d='M 442.13 38.011 C 473.779 89.661 403.778 187.807 285.777 257.224 C 167.775 326.642 46.459 341.045 14.809 289.394 C -16.841 237.744 53.161 139.599 171.162 70.182 C 289.162 0.764 410.48 -13.639 442.13 38.011 Z'/%3E%3Cpath style='fill:%23FFDC0F;stroke:%23000000;stroke-width:10;' d='M 63.688 87.538 C 334.367 1.309 278.975 125.798 215.017 137.791 L 229.179 90.965 L 141.922 114.378 L 53.98 341.657 L 148.775 311.391 L 193.888 189.186 L 381.765 354.219 L 295.536 151.495 C 468.565 53.274 383.604 -58.652 79.107 68.122 L 63.688 87.538 Z'/%3E%3C/svg%3E";

    /// <summary>
    /// Registers and configures OpenAPI services from the application's configuration.
    /// </summary>
    /// <remarks>
    /// This method binds the <see cref="OpenApiInfoSettings"/> from the configuration section specified by <see cref="OpenApiInfoSettings.AppSettingsKey"/>
    /// and adds the necessary OpenAPI services to the dependency injection container.
    /// <para>
    /// It leverages <c>RA.Utilities.OpenApi</c> to add default document transformers which:
    /// <list type="bullet">
    ///   <item><description>Populate document info (title, version, etc.) from configuration.</description></item>
    ///   <item><description>Add a Bearer token security scheme for JWT authentication.</description></item>
    ///   <item><description>Add a standard <c>x-request-id</c> header parameter to all operations.</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The application's <see cref="IConfiguration"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddOpenApi(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<OpenApiInfoSettings>(configuration.GetSection(OpenApiInfoSettings.AppSettingsKey))
            .AddOpenApi(options => options.AddDefaultsDocumentTransformer());

        return services;
    }

    /// <summary>
    /// Configures OpenAPI and API documentation UI (Scalar or Swagger UI) endpoints.
    /// </summary>
    /// <remarks>
    /// This method maps the OpenAPI specification endpoint and the selected API documentation UI.
    /// It sets various UI options, such as title and authentication settings.
    /// The title is read from the "OpenApiInfoSettings:Title" configuration key.
    /// <para>
    /// When authorization is enabled, both UIs are configured for Bearer authentication:
    /// the Bearer security scheme is already part of the OpenAPI document (added by <c>RA.Utilities.OpenApi</c>),
    /// Scalar gets an explicit Bearer security scheme entry, and Swagger UI keeps the entered
    /// token across page reloads via <c>persistAuthorization</c>.
    /// </para>
    /// </remarks>
    /// <param name="app">The <see cref="WebApplication"/> to add the endpoints to.</param>
    public static void UseOpenApi(this WebApplication app)
    {
        app.MapOpenApi();

        OpenApiInfoSettings openApiSettings =
            app.Services.GetRequiredService<IOptions<OpenApiInfoSettings>>().Value;

        app.MapScalarApiReference("/openapi-ui", options =>
        {
            options.Title = openApiSettings.Title ?? Title;
            options.DarkMode = false;
            options.Favicon = FavIcon;
            options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.Shell, ScalarClient.Curl);
            options.HideModels = false;
            options.Layout = ScalarLayout.Modern;
            options.ShowSidebar = true;
            options.OrderRequiredPropertiesFirst = true;
        });
    }
}
