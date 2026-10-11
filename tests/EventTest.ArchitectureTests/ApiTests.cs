using System.Reflection;
using Microsoft.AspNetCore.Http;
using NetArchTest.Rules;
using RA.Utilities.Api.Abstractions;
using Xunit;

namespace EventTest.ArchitectureTests;

/// <summary>
/// Contains architecture tests for API layer.
/// </summary>
public class ApiTests
{
    private const string MediatorNameSpace = "RA.Utilities.Feature.Abstractions";

    /// <summary>
    /// Verifies that endpoints depend on the mediator abstraction.
    /// </summary>
    [Fact]
    public void Endpoints_Should_HaveDependencyOnMediator()
    {
        // Act
        TestResult testResult = Types
            .InAssembly(Assemblies.Application)
            .That()
            .ImplementInterface(typeof(IEndpoint))
            .Should()
            .HaveDependencyOn(MediatorNameSpace)
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }

    /// <summary>
    /// Verifies that endpoints return typed results containing API contract objects.
    /// </summary>
    [Fact]
    public void Endpoints_Should_ReturnTypedResultsWithApiContracts()
    {
        Type[] endpointTypes = new[] { Assemblies.Api, Assemblies.Application }
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && typeof(IEndpoint).IsAssignableFrom(type))
            .ToArray();

        string[] invalidEndpoints = endpointTypes
            .SelectMany(endpointType => endpointType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(method => !method.IsSpecialName)
                .Select(method => new { endpointType, method }))
            .Where(endpoint => !ReturnsTypedApiContract(endpoint.method.ReturnType))
            .Select(endpoint => $"{endpoint.endpointType.FullName}.{endpoint.method.Name}")
            .ToArray();

        Assert.True(
            invalidEndpoints.Length == 0,
            $"Endpoints must return a typed result containing an Api.Contracts object, unless they return NoContent or Redirect: {string.Join(", ", invalidEndpoints)}");
    }

    private static bool ReturnsTypedApiContract(Type returnType)
    {
        Type responseType = UnwrapAsyncResult(returnType);

        if (ContainsNoContentOrRedirect(responseType))
        {
            return true;
        }

        return typeof(IResult).IsAssignableFrom(responseType)
            && ContainsApiContractType(responseType);
    }

    private static Type UnwrapAsyncResult(Type returnType)
    {
        if (returnType.IsGenericType
            && (returnType.GetGenericTypeDefinition() == typeof(Task<>)
                || returnType.GetGenericTypeDefinition() == typeof(ValueTask<>)))
        {
            return returnType.GetGenericArguments()[0];
        }

        return returnType;
    }

    private static bool ContainsNoContentOrRedirect(Type type)
    {
        if (type.Name.Contains("NoContent", StringComparison.Ordinal)
            || type.Name.Contains("Redirect", StringComparison.Ordinal))
        {
            return true;
        }

        return type.IsGenericType
            && type.GetGenericArguments().Any(ContainsNoContentOrRedirect);
    }

    private static bool ContainsApiContractType(Type type)
    {
        if (type.Assembly == Assemblies.ApiContracts && type.IsClass && !type.IsAbstract)
        {
            return true;
        }

        return type.IsGenericType
            && type.GetGenericArguments().Any(ContainsApiContractType);
    }
}
