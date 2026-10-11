using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RA.Utilities.Data.EntityFramework.Interceptors;
using EventTest.Application.Abstractions.Data;
using EventTest.Persistence.Database;

namespace EventTest.Persistence;

/// <summary>
/// Provides dependency injection for persistence services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds persistence services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add the services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration" /> containing application settings.</param>
    /// <returns>The <see cref="IServiceCollection" /> so that additional services can be chained.</returns>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration)
            .AddHealthChecks(configuration);

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<BaseEntitySaveChangesInterceptor>();

        services.AddDbContext<EventTestSqliteDbContext>((provider, options) =>
        {
            options.UseSqlite(GetConnectionString(configuration, "EventTestSqliteConnectionString"));
            ConfigureOptions<EventTestSqliteDbContext>(provider, options);
        });
        services.AddScoped<IEventTestSqliteDbContext>(sp => sp.GetRequiredService<EventTestSqliteDbContext>());

        return services;
    }

    private static void AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        IHealthChecksBuilder healthChecksBuilder = services.AddHealthChecks();

        healthChecksBuilder.AddSqlite(GetConnectionString(configuration, "EventTestSqliteConnectionString"));
    }

    private static void ConfigureOptions<TContext>(IServiceProvider provider, DbContextOptionsBuilder options)
        where TContext : DbContext
    {
        options.AddInterceptors(provider.GetRequiredService<BaseEntitySaveChangesInterceptor>());

        IHostEnvironment env = provider.GetRequiredService<IHostEnvironment>();

        if (env.IsDevelopment())
        {
            ILogger<TContext> logger = provider.GetRequiredService<ILogger<TContext>>();
            options.LogTo(
                    msg =>
                    {
                        if (logger.IsEnabled(LogLevel.Information))
                        {
                            logger.LogInformation("{Message}", msg);
                        }
                    },
                    [DbLoggerCategory.Database.Command.Name],
                    LogLevel.Information)
                .EnableSensitiveDataLogging();
        }
    }

    private static string GetConnectionString(IConfiguration configuration, string name)
    {
        string? connectionString = configuration.GetConnectionString(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        return connectionString;
    }
}
