using NetArchTest.Rules;
using Xunit;

namespace EventTest.ArchitectureTests;

/// <summary>
/// Contains architecture tests to verify project dependencies and layer isolation.
/// </summary>
public class DependencyTests
{
    /// <summary>
    /// Verifies that the API Contracts layer does not depend on any other projects in the solution.
    /// </summary>
    [Fact]
    public void ApiContracts_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange

        string[] otherProjects =
        [
            Namespaces.Domain,
            Namespaces.Application,
            Namespaces.Infrastructure,
            // Namespaces.Api is intentionally not listed: NetArchTest matches dependencies
            // by namespace prefix, so it would also match this assembly's own EventTest.Api.Contracts namespaces.
            Namespaces.Integration,
            Namespaces.Persistence,
        ];

        // Act
        TestResult testResult = Types
            .InAssembly(Assemblies.ApiContracts)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }

    /// <summary>
    /// Verifies that the Domain layer does not depend on any other projects in the solution.
    /// </summary>
    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange

        string[] otherProjects =
        [
            Namespaces.Application,
            Namespaces.Infrastructure,
            Namespaces.Api,
            Namespaces.ApiContracts,
            Namespaces.Integration,
            Namespaces.Persistence,
        ];

        // Act
        TestResult testResult = Types
            .InAssembly(Assemblies.Domain)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }

    /// <summary>
    /// Verifies that the Application layer does not depend on forbidden projects in the solution.
    /// </summary>
    [Fact]
    public void Application_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange

        string[] otherProjects =
        [
            Namespaces.Api,
            Namespaces.ApiContracts,
            Namespaces.Infrastructure,
            Namespaces.Integration,
            Namespaces.Persistence,
        ];

        // Act
        TestResult testResult = Types
            .InAssembly(Assemblies.Application)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }

    /// <summary>
    /// Verifies that the Infrastructure layer does not depend on forbidden projects in the solution.
    /// </summary>
    [Fact]
    public void Infrastructure_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange

        string[] otherProjects =
        [
            Namespaces.Api,
            Namespaces.ApiContracts,
        ];

        // Act
        TestResult testResult = Types
            .InAssembly(Assemblies.Infrastructure)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }

    /// <summary>
    /// Verifies that the Persistence layer does not depend on forbidden projects in the solution.
    /// </summary>
    [Fact]
    public void Persistence_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange

        string[] otherProjects =
        [
            Namespaces.Api,
            Namespaces.ApiContracts,
            Namespaces.Infrastructure,
            Namespaces.Integration,
        ];

        // Act
        TestResult testResult = Types
            .InAssembly(Assemblies.Persistence)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }

    /// <summary>
    /// Verifies that the Integration layer does not depend on forbidden projects in the solution.
    /// </summary>
    [Fact]
    public void Integration_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange

        string[] otherProjects =
        [
            Namespaces.Api,
            Namespaces.ApiContracts,
            Namespaces.Infrastructure,
            Namespaces.Persistence,
        ];

        // Act
        TestResult testResult = Types
            .InAssembly(Assemblies.Integration)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }
}
