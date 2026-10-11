using System;
using FluentValidation;
using NetArchTest.Rules;
using RA.Utilities.Feature.Abstractions;
using Xunit;

namespace EventTest.ArchitectureTests;

/// <summary>
/// Contains architecture tests for the application layer.
/// </summary>
public class ApplicationTests
{
    private const string RequestSuffix = "Input";
    private const string HandlerSuffix = "Handler";
    private const string OutputSuffix = "Output";
    private const string ValidatorSuffix = "Validator";
    private const string DecoratorSuffix = "Decorator";

    /// <summary>
    /// Verifies that all feature inputs have the 'Input' suffix.
    /// </summary>
    [Fact]
    public void FeatureInputs_Should_Have_Input_Suffix()
    {
        // Act
        TestResult result = Types
            .InAssembly(Assemblies.Application)
            .That()
            .ImplementInterface(typeof(IRequest))
            .Or()
            .ImplementInterface(typeof(IRequest<>))
            .Should()
            .HaveNameEndingWith(RequestSuffix)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Verifies that feature outputs used in IRequestHandler have the 'Output' Suffix.
    /// </summary>
    [Fact]
    public void FeatureOutputs_Should_Have_Output_Suffix()
    {
        IEnumerable<Type> handlerTypes = Types.InAssembly(Assemblies.Application)
            .That()
            .AreClasses()
            .And()
            .AreNotAbstract()
            .And()
            .ImplementInterface(typeof(IRequestHandler<>))
            .Or()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .GetTypes();

        IEnumerable<string?> failingTypes = handlerTypes.Where(handlerType =>
        {
            Type? featureHandlerInterface = handlerType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && (
                    i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                    i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                );
            Type? outputType = featureHandlerInterface?.GetGenericArguments().ElementAtOrDefault(1);

            if (outputType is null)
            {
                return false;
            }

            if (outputType.IsGenericType)
            {
                outputType = outputType.GetGenericArguments().FirstOrDefault();
            }

            return outputType != null && !outputType.Name.EndsWith(OutputSuffix, StringComparison.OrdinalIgnoreCase);
        })
        .Select(t => t.FullName);

        Assert.False(
            failingTypes.Any(),
            $"The following feature handlers have output types that do not end with '{OutputSuffix}': {string.Join(", ", failingTypes)}"
        );
    }

    /// <summary>
    /// Verifies that all feature handlers have the 'Handler' Suffix.
    /// </summary>
    [Fact]
    public void FeatureHandlers_Should_Have_Handler_Suffix()
    {
        TestResult result = Types.InAssembly(Assemblies.Application)
            .That()
            .ImplementInterface(typeof(IRequestHandler<>))
            .Or()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .And()
            .AreNotAbstract()
            .Should()
            .HaveNameEndingWith(HandlerSuffix)
            .Or()
            .HaveNameEndingWith(DecoratorSuffix)
            .Or()
            .HaveNameEndingWith(ValidatorSuffix)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// All Features Decorator should have 'Decorator' suffix.
    /// </summary>
    [Fact]
    public void FeatureDecorators_Should_Have_Decorator_Suffix()
    {
        TestResult result = Types.InAssembly(Assemblies.Application)
            .That()
            .ImplementInterface(typeof(IPipelineBehavior<>))
            .Or()
            .ImplementInterface(typeof(IPipelineBehavior<,>))
            .Should()
            .HaveNameEndingWith(DecoratorSuffix)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Verifies that all feature validators have the 'Validator' Suffix.
    /// </summary>
    [Fact]
    public void FeatureValidators_Should_Have_Validator_Suffix()
    {
        TestResult result = Types.InAssembly(Assemblies.Application)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .Should()
            .HaveNameEndingWith(ValidatorSuffix)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Verifies that all feature inputs are sealed.
    /// </summary>
    [Fact]
    public void FeatureInputs_Should_BeSealed()
    {
        TestResult result = Types.InAssembly(Assemblies.Application)
            .That()
            .ImplementInterface(typeof(IRequest))
            .Or()
            .ImplementInterface(typeof(IRequest<>))
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Verifies that all feature handlers are sealed.
    /// </summary>
    [Fact]
    public void FeatureHandlers_Should_BeSealed()
    {
        TestResult result = Types.InAssembly(Assemblies.Application)
            .That()
            .ImplementInterface(typeof(IRequestHandler<>))
            .Or()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .And()
            .AreNotAbstract()
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Verifies that all feature validators are sealed.
    /// </summary>
    [Fact]
    public void FeatureValidators_Should_BeSealed()
    {
        TestResult result = Types.InAssembly(Assemblies.Application)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Verifies that all feature decorators are sealed.
    /// </summary>
    [Fact]
    public void FeatureDecorators_Should_BeSealed()
    {
        TestResult result = Types.InAssembly(Assemblies.Application)
            .That()
            .ImplementInterface(typeof(IPipelineBehavior<>))
            .Or()
            .ImplementInterface(typeof(IPipelineBehavior<,>))
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }
}
