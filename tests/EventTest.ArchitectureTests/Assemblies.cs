using System.Reflection;

namespace EventTest.ArchitectureTests;

internal static class Assemblies
{
    public static readonly Assembly Domain = typeof(Domain.AssemblyReference).Assembly;
    public static readonly Assembly Application = typeof(Application.AssemblyReference).Assembly;
    public static readonly Assembly Infrastructure = typeof(Infrastructure.AssemblyReference).Assembly;

    public static readonly Assembly Integration = typeof(Integration.AssemblyReference).Assembly;
    public static readonly Assembly Persistence = typeof(Persistence.AssemblyReference).Assembly;
    public static readonly Assembly Api = typeof(Api.AssemblyReference).Assembly;
    public static readonly Assembly ApiContracts = typeof(Api.Contracts.AssemblyReference).Assembly;
}
