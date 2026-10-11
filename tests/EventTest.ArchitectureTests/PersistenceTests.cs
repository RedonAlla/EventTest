using Microsoft.EntityFrameworkCore;
using NetArchTest.Rules;
using Xunit;

namespace EventTest.ArchitectureTests;

/// <summary>
/// Contains architecture tests for the <strong>Persistence</strong> layer.
/// </summary>
public class PersistenceTests
{
    private const string EntityConfigurationsSuffix = "Config";
    private const string EntityConfigurationsNamespace = "EventTest.Persistence.Configuration";

    /// <summary>
    /// Verifies that all entities databases configuration internal are sealed.
    /// </summary>
    [Fact]
    public void EntityConfigurations_Should_InternalSealedClasses()
    {
        // Act
        TestResult testResult = Types
            .InAssembly(Assemblies.Persistence)
            .That()
            .ImplementInterface(typeof(IEntityTypeConfiguration<>))
            .Should()
            .NotBePublic() //TODO Find a way to ensure to be internal
            .And()
            .BeSealed()
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }

    //TODO test EntityConfigurations_Should_HaveConfigSuffix if it works
    // If it works remove this one
    /// <summary>
    /// Verifies that all entities databases configurations ends with <strong>Config</strong> suffix.
    /// </summary>
    [Fact]
    public void EntityConfigurations_Should_HaveConfigSuffix()
    {
        // Act
        TestResult result = Types
            .InAssembly(Assemblies.Persistence)
            .That()
            .ImplementInterface(typeof(IEntityTypeConfiguration<>))
            .Should()
            .HaveNameEndingWith(EntityConfigurationsSuffix)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Verifies that all entities databases configuration are under Persistence layer and under Configuration directory.
    /// </summary>
    [Fact]
    public void All_EntityConfigurations_At_PersistenceConfiguration_Directory()
    {
        TestResult result = Types
            .InAssembly(Assemblies.Persistence)
            .That()
            .ImplementInterface(typeof(IEntityTypeConfiguration<>))
            .Should()
            .ResideInNamespace(EntityConfigurationsNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Verifies that all entities databases configurations follow the naming convention:
    /// the entity type name of <see cref="IEntityTypeConfiguration{TEntity}"/> plus a
    /// <strong>Config</strong> suffix (e.g. <c>IEntityTypeConfiguration&lt;User&gt;</c> must be named <c>UserConfig</c>).
    /// </summary>
    [Fact]
    public void EntityConfigurations_NamingConvention()
    {
        // Act
        IEnumerable<Type> nonConformingConfigurations = Types
            .InAssembly(Assemblies.Persistence)
            .That()
            .ImplementInterface(typeof(IEntityTypeConfiguration<>))
            .GetTypes()
            .Where(configurationType =>
            {
                string name = GetEntityName(configurationType);
                return configurationType.Name != $"{name}{EntityConfigurationsSuffix}";
            });

        // Assert
        Assert.Empty(nonConformingConfigurations);
    }

    private string GetEntityName(Type configurationType)
    {
        Type entityType = configurationType
            .GetInterfaces()
            .Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
            .GetGenericArguments()[0];

        return entityType.Name;
    }
}
