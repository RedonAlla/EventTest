using FluentValidation;
using NetArchTest.Rules;
using RA.Utilities.Feature.Abstractions;
using Xunit;

namespace EventTest.ArchitectureTests;

/// <summary>
/// Contains architecture tests for handlers.
/// </summary>
public class HandlersTests
{
    private const string FeatureInputPostfix = "Input";
    private const string FeatureHandlerPostfix = "Handler";
    private const string FeatureOutputPostfix = "Output";
    private const string FeatureValidatorPostfix = "Validator";
    private const string FeatureDecoratorPostfix = "Decorator";

    /// <summary>
    /// Verifies that all feature inputs have the 'Input' postfix.
    /// </summary>
    [Fact]
    public void FeatureInputs_Should_HaveInputPostfix()
    {
        TestResult result = Types.InAssembly(Assemblies.Application)
            .That()
            .ImplementInterface(typeof(IRequest))
            .Or()
            .ImplementInterface(typeof(IRequest<>))
            .Should()
            .HaveNameEndingWith(FeatureInputPostfix)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Verifies that feature outputs used in IRequestHandler have the 'Output' postfix.
    /// </summary>
    [Fact]
    public void FeatureOutputs_Should_Have_Output_Postfix()
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

            return outputType != null && !outputType.Name.EndsWith(FeatureOutputPostfix, StringComparison.OrdinalIgnoreCase);
        })
        .Select(t => t.FullName);

        Assert.Empty(failingTypes);
    }

    /// <summary>
    /// Verifies that all feature handlers have the 'Handler' postfix.
    /// </summary>
    [Fact]
    public void FeatureHandlers_Should_Have_Handler_Postfix()
    {
        TestResult result = Types.InAssembly(Assemblies.Application)
            .That()
            .ImplementInterface(typeof(IRequestHandler<>))
            .Or()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .And()
            .AreNotAbstract()
            .Should()
            .HaveNameEndingWith(FeatureHandlerPostfix)
            .Or()
            .HaveNameEndingWith(FeatureDecoratorPostfix)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Verifies that all feature validators have the 'Validator' postfix.
    /// </summary>
    [Fact]
    public void FeatureValidators_Should_Have_Validator_Postfix()
    {
        TestResult result = Types.InAssembly(Assemblies.Application)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .Should()
            .HaveNameEndingWith(FeatureValidatorPostfix)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    /// <summary>
    /// Verifies that all feature inputs have the 'Input' postfix.
    /// </summary>
    [Fact]
    public void Decorator_Should_HaveDecoratorPostfix()
    {
        TestResult result = Types.InAssembly(Assemblies.Application)
            .That()
            .Inherit(typeof(INotificationHandler<>))
            .Or()
            .Inherit(typeof(IPipelineBehavior<>))
            .Or()
            .Inherit(typeof(IPipelineBehavior<,>))
            .Should()
            .HaveNameEndingWith(FeatureDecoratorPostfix)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}
