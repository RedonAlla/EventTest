using NetArchTest.Rules;
using RA.Utilities.Data.Entities;
using Xunit;

namespace EventTest.ArchitectureTests;

/// <summary>
/// Contains architecture tests for domain entities.
/// </summary>
public class EntitiesTests
{
    private const string EntitiesNamespace = "EventTest.Domain.Entities";

    /// <summary>
    /// Verifies that all domain entities inherit from <see cref="CoreEntity{TKey}"/>.
    /// </summary>
    [Fact]
    public void All_Entities_Should_Inherit_From_CoreEntity()
    {
        TestResult result = Types.InAssembly(Assemblies.Domain)
            .That()
            .ResideInNamespace(EntitiesNamespace)
            .Should()
            .Inherit(typeof(CoreEntity<>))
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Verifies that all domain entities are sealed.
    /// </summary>
    [Fact]
    public void Entities_Should_Be_Sealed()
    {
        TestResult result = Types.InAssembly(Assemblies.Domain)
            .That()
            .Inherit(typeof(CoreEntity<>))
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Verifies that only types residing in the entities namespace inherit from <see cref="CoreEntity{TKey}"/>.
    /// </summary>
    [Fact]
    public void Only_Entities_Should_Inherit_From_BaseEntity()
    {
        TestResult result = Types.InAssembly(Assemblies.Domain)
            .That()
            .Inherit(typeof(CoreEntity<>))
            .Should()
            .ResideInNamespace(EntitiesNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }
}
