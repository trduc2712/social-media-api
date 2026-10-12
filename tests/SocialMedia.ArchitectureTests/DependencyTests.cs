using System.Reflection;
using System.Runtime.InteropServices;

namespace SocialMedia.ArchitectureTests;

public class DependencyTests
{
    private static readonly Assembly Domain = SocialMedia.Domain.AssemblyReference.Assembly;
    private static readonly Assembly Contract = SocialMedia.Contract.AssemblyReference.Assembly;
    private static readonly Assembly Application = SocialMedia.Application.AssemblyReference.Assembly;
    private static readonly Assembly Infrastructure = SocialMedia.Infrastructure.AssemblyReference.Assembly;
    private static readonly Assembly Api = SocialMedia.Api.AssemblyReference.Assembly;

    private static readonly Assembly[] Projects = [Domain, Contract, Application, Infrastructure, Api];

    private static readonly string FrameworkDirectory =
        Path.TrimEndingDirectorySeparator(RuntimeEnvironment.GetRuntimeDirectory());

    public static TheoryData<string, string> ForbiddenProjectReferences => new()
    {
        { NameOf(Domain), NameOf(Contract) },
        { NameOf(Domain), NameOf(Application) },
        { NameOf(Domain), NameOf(Infrastructure) },
        { NameOf(Domain), NameOf(Api) },
        { NameOf(Contract), NameOf(Domain) },
        { NameOf(Contract), NameOf(Application) },
        { NameOf(Contract), NameOf(Infrastructure) },
        { NameOf(Contract), NameOf(Api) },
        { NameOf(Application), NameOf(Infrastructure) },
        { NameOf(Application), NameOf(Api) },
        { NameOf(Infrastructure), NameOf(Api) },
    };

    public static TheoryData<string> FrameworkOnlyProjects => [NameOf(Domain), NameOf(Contract)];

    [Theory]
    [MemberData(nameof(ForbiddenProjectReferences))]
    public void Project_ShouldNotReference_ForbiddenProject(string project, string forbidden)
    {
        Assert.DoesNotContain(forbidden, ReferencesOf(project));
    }

    [Theory]
    [MemberData(nameof(FrameworkOnlyProjects))]
    public void Project_ShouldReferenceOnlyTheFramework(string project)
    {
        Assert.DoesNotContain(AssemblyOf(project).GetReferencedAssemblies(), reference => !IsFrameworkAssembly(reference));
    }

    private static string NameOf(Assembly assembly) => assembly.GetName().Name!;

    private static Assembly AssemblyOf(string project) =>
        Projects.Single(assembly => NameOf(assembly) == project);

    private static IEnumerable<string> ReferencesOf(string project) =>
        AssemblyOf(project).GetReferencedAssemblies().Select(reference => reference.Name!);

    private static bool IsFrameworkAssembly(AssemblyName reference) =>
        Path.GetDirectoryName(Assembly.Load(reference).Location) == FrameworkDirectory;
}
