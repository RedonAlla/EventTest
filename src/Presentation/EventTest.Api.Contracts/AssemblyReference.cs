using System.Diagnostics.CodeAnalysis;

namespace EventTest.Api.Contracts;

/// <summary>
///     Marker type used by the composition root to locate the ApiContracts assembly.
/// </summary>
[SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "The type is intentionally public so the composition root can reference the ApiContracts assembly.")]
public sealed class AssemblyReference;
