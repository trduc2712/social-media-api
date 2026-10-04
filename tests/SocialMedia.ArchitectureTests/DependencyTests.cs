using System.Reflection;

namespace SocialMedia.ArchitectureTests;

public class DependencyTests
{
    private static readonly Assembly Domain = SocialMedia.Domain.AssemblyReference.Assembly;
    private static readonly Assembly Contract = SocialMedia.Contract.AssemblyReference.Assembly;
    private static readonly Assembly Application = SocialMedia.Application.AssemblyReference.Assembly;
    private static readonly Assembly Infrastructure = SocialMedia.Infrastructure.AssemblyReference.Assembly;
    private static readonly Assembly Api = SocialMedia.Api.AssemblyReference.Assembly;

    private static readonly Assembly[] Projects = [Domain, Contract, Application, Infrastructure, Api];

    private static readonly string[] ForbiddenPackages = ["Microsoft.EntityFrameworkCore", "Npgsql"];

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

    public static TheoryData<string, string> ForbiddenPackageReferences
    {
        get
        {
            var data = new TheoryData<string, string>();

            foreach (var project in new[] { Domain, Contract })
            {
                foreach (var package in ForbiddenPackages)
                {
                    data.Add(NameOf(project), package);
                }
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(ForbiddenProjectReferences))]
    public void Project_ShouldNotReference_ForbiddenProject(string project, string forbidden)
    {
        Assert.DoesNotContain(forbidden, ReferencesOf(project));
    }

    [Theory]
    [MemberData(nameof(ForbiddenPackageReferences))]
    public void ProjectShouldNotReference_ForbiddenPackage(string project, string package)
    {
        Assert.DoesNotContain(
            ReferencesOf(project),
            reference => reference == package || reference.StartsWith(package + ".", StringComparison.Ordinal));
    }

    private static string NameOf(Assembly assembly) => assembly.GetName().Name!;

    private static IEnumerable<string> ReferencesOf(string project) =>
        Projects
            .Single(assembly => NameOf(assembly) == project)
            .GetReferencedAssemblies()
            .Select(reference => reference.Name!);
}
