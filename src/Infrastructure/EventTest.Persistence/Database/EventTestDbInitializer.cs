#pragma warning disable S1144
#pragma warning disable S1172

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventTest.Persistence.Database;

/// <summary>
/// Provides extension methods for database initialization.
/// </summary>
public static class EventTestDbInitializer
{
    /// <summary>
    /// Initializes and seeds the application databases.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider" /> used to resolve the database initializers.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        await InitializeAsync(
            scope.ServiceProvider.GetRequiredService<EventTestSqliteDbContext>(),
            scope.ServiceProvider.GetRequiredService<ILogger<EventTestSqliteDbContext>>());
    }

    /// <summary>
    /// Creates the database if it does not exist.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    private static async Task InitializeAsync<TContext>(TContext context, ILogger<TContext> logger)
        where TContext : DbContext
    {
        try
        {
            // See https://jasontaylor.dev/ef-core-database-initialisation-strategies
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            if (logger.IsEnabled(LogLevel.Information))
            {
                string sql = context.Database.GenerateCreateScript();
                logger.LogInformation("======================= Data Base {DbContext} script =======================", typeof(TContext).Name);
                logger.LogInformation("{SqlScript}", sql);
                logger.LogInformation("============================================================================");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    /// <summary>
    /// Seeds the database with default data.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    private static async Task SeedAsync<TContext>(TContext context, ILogger<TContext> logger)
        where TContext : DbContext
    {
        try
        {
            // Default data
            // Seed, if necessary
            // if (!context.TodoLists.Any())
            // {
            //     context.TodoLists.Add(new TodoList
            //     {
            //     });

            //     await context.SaveChangesAsync();
            // }

            //logger.LogInformation("Seeded {Count} sample todos", todos.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}
