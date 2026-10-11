using System.Diagnostics.CodeAnalysis;

namespace EventTest.Api;

/// <summary>
///     Marker type used by the composition root to locate the Api assembly.
/// </summary>
[SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "The type is intentionally public so the composition root can reference the Api assembly.")]
public sealed class AssemblyReference;
