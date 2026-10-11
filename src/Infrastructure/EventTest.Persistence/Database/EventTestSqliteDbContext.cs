using Microsoft.EntityFrameworkCore;
using EventTest.Application.Abstractions.Data;

namespace EventTest.Persistence.Database;

/// <summary>
/// Represents the SQLite database context.
/// </summary>
public sealed class EventTestSqliteDbContext(DbContextOptions<EventTestSqliteDbContext> options)
    : DbContext(options), IEventTestSqliteDbContext
{
    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventTestSqliteDbContext).Assembly);
    }
}
