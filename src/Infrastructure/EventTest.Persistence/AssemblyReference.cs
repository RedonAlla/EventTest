using System.Diagnostics.CodeAnalysis;

namespace EventTest.Persistence;

/// <summary>
///     Marker type used by the composition root to locate the Persistence assembly.
/// </summary>
[SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "The type is intentionally public so the composition root can reference the Persistence assembly.")]
public sealed class AssemblyReference;
