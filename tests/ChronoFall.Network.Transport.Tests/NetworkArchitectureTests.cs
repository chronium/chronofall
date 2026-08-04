using System.Xml.Linq;

namespace ChronoFall.Network.Transport.Tests;

public sealed class NetworkArchitectureTests
{
    private static string RepositoryRoot { get; } = FindRepositoryRoot();

    [Fact]
    public void ContractsProjectIsBclOnly()
    {
        XDocument contracts = LoadProject(
            "src",
            "ChronoFall.Network.Transport",
            "ChronoFall.Network.Transport.csproj");

        Assert.Empty(contracts.Descendants("ProjectReference"));
        Assert.Empty(contracts.Descendants("PackageReference"));
    }

    [Fact]
    public void AdapterIsTheOnlySharedProjectThatReferencesLiteNetLibSource()
    {
        XDocument adapter = LoadProject(
            "src",
            "ChronoFall.Network.Transport.LiteNetLib",
            "ChronoFall.Network.Transport.LiteNetLib.csproj");
        XElement[] references = adapter.Descendants("ProjectReference").ToArray();

        Assert.Equal(2, references.Length);
        Assert.Contains(
            references,
            reference => reference.Attribute("Include")?.Value ==
                "../ChronoFall.Network.Transport/ChronoFall.Network.Transport.csproj");

        XElement liteNetLib = Assert.Single(
            references,
            reference => reference.Attribute("Include")?.Value ==
                "../../thirdparty/repos/LiteNetLib/LiteNetLib/LiteNetLib.csproj");
        Assert.Equal(
            "GeneratePackageOnBuild=false",
            liteNetLib.Attribute("AdditionalProperties")?.Value);

        string[] otherSharedProjects = Directory
            .EnumerateFiles(Path.Combine(RepositoryRoot, "src"), "*.csproj", SearchOption.AllDirectories)
            .Where(path => !path.EndsWith(
                "ChronoFall.Network.Transport.LiteNetLib.csproj",
                StringComparison.Ordinal))
            .Where(path => File.ReadAllText(path).Contains("LiteNetLib.csproj", StringComparison.Ordinal))
            .ToArray();
        Assert.Empty(otherSharedProjects);
    }

    [Fact]
    public void SharedNetworkSourceRemainsChildIndependentAndGraphicsFree()
    {
        string[] forbidden = ["Royale", "Starfall", "SDL", "ImGui", "Gpu", "Rendering"];
        string[] roots =
        [
            Path.Combine(RepositoryRoot, "src", "ChronoFall.Network.Transport"),
            Path.Combine(RepositoryRoot, "src", "ChronoFall.Network.Transport.LiteNetLib"),
        ];

        foreach (string file in roots.SelectMany(root =>
                     Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
                         .Where(path => Path.GetExtension(path) is ".cs" or ".csproj")))
        {
            string content = File.ReadAllText(file);
            foreach (string fragment in forbidden)
            {
                Assert.DoesNotContain(fragment, content, StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    [Fact]
    public void FamilyConsumerDirectlyReferencesOnlyAdapterThroughFamilyRoot()
    {
        XDocument consumer = LoadProject(
            "tests",
            "ChronoFall.NetworkTransport.FamilySourceConsumer",
            "ChronoFall.NetworkTransport.FamilySourceConsumer.csproj");
        XElement reference = Assert.Single(consumer.Descendants("ProjectReference"));

        Assert.Equal(
            "$(ChronoFallFamilyRoot)src/ChronoFall.Network.Transport.LiteNetLib/ChronoFall.Network.Transport.LiteNetLib.csproj",
            reference.Attribute("Include")?.Value);
        Assert.Empty(consumer.Descendants("PackageReference"));
    }

    [Fact]
    public void ThirdPartyPinAndNoPackageBuildPolicyAreExplicit()
    {
        string versions = File.ReadAllText(Path.Combine(RepositoryRoot, "thirdparty", "versions.env"));
        string targets = File.ReadAllText(Path.Combine(RepositoryRoot, "thirdparty", "Directory.Build.targets"));

        Assert.Contains(
            "LITENETLIB_COMMIT=\"37cbf5ab608a4dbd0e491c528a0c14c1e09f1cba\"",
            versions,
            StringComparison.Ordinal);
        Assert.Contains("'$(MSBuildProjectName)' == 'LiteNetLib'", targets, StringComparison.Ordinal);
        Assert.Contains("<GeneratePackageOnBuild>false</GeneratePackageOnBuild>", targets, StringComparison.Ordinal);
    }

    private static XDocument LoadProject(params string[] path) =>
        XDocument.Load(Path.Combine([RepositoryRoot, .. path]));

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "ChronoFall.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the ChronoFall repository root.");
    }
}
