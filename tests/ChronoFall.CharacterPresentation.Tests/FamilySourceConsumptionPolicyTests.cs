using System.Xml.Linq;

namespace ChronoFall.CharacterPresentation.Tests;

public sealed class FamilySourceConsumptionPolicyTests
{
    private static string RepositoryRoot { get; } = FindRepositoryRoot();

    [Fact]
    public void FamilyRootDefaultsToCoordinatorRoot()
    {
        XDocument properties = XDocument.Load(Path.Combine(RepositoryRoot, "Directory.Build.props"));
        XElement root = Assert.Single(properties.Descendants("ChronoFallFamilyRoot"));

        Assert.Equal("'$(ChronoFallFamilyRoot)' == ''", root.Attribute("Condition")?.Value);
        Assert.Equal("$([MSBuild]::NormalizeDirectory('$(MSBuildThisFileDirectory)'))", root.Value);
    }

    [Fact]
    public void SmokeConsumerUsesOnlyTheApprovedFamilyRootReferences()
    {
        XDocument project = XDocument.Load(Path.Combine(
            RepositoryRoot,
            "tests",
            "ChronoFall.FamilySourceConsumer",
            "ChronoFall.FamilySourceConsumer.csproj"));
        string[] references = project.Descendants("ProjectReference")
            .Select(reference => reference.Attribute("Include")?.Value)
            .Where(static value => value is not null)
            .Cast<string>()
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
        [
            "$(ChronoFallFamilyRoot)src/ChronoFall.CharacterPresentation.Cooking/ChronoFall.CharacterPresentation.Cooking.csproj",
            "$(ChronoFallFamilyRoot)src/ChronoFall.CharacterPresentation.SdlGpu/ChronoFall.CharacterPresentation.SdlGpu.csproj",
            "$(ChronoFallFamilyRoot)src/ChronoFall.CharacterPresentation/ChronoFall.CharacterPresentation.csproj",
        ],
        references);
        Assert.Empty(project.Descendants("PackageReference"));
    }

    [Fact]
    public void SdlGpuBuildsCheckedOutSdlSourceAndUsesOnlyTheApprovedPngPackage()
    {
        XDocument project = XDocument.Load(Path.Combine(
            RepositoryRoot,
            "src",
            "ChronoFall.CharacterPresentation.SdlGpu",
            "ChronoFall.CharacterPresentation.SdlGpu.csproj"));
        string[] references = project.Descendants("ProjectReference")
            .Select(reference => reference.Attribute("Include")?.Value)
            .Where(static value => value is not null)
            .Cast<string>()
            .ToArray();

        Assert.Contains("../../thirdparty/repos/SDL3-CS/SDL3-CS/SDL3-CS.csproj", references);
        XElement package = Assert.Single(project.Descendants("PackageReference"));
        Assert.Equal("StbImageWriteSharp", package.Attribute("Include")?.Value);

        XDocument packages = XDocument.Load(Path.Combine(RepositoryRoot, "Directory.Packages.props"));
        XElement version = Assert.Single(
            packages.Descendants("PackageVersion"),
            static candidate => candidate.Attribute("Include")?.Value == "StbImageWriteSharp");
        Assert.Equal("1.16.7", version.Attribute("Version")?.Value);

        string provenance = File.ReadAllText(Path.Combine(
            RepositoryRoot,
            "thirdparty",
            "licenses",
            "StbImageWriteSharp",
            "PROVENANCE.md"));
        Assert.Contains("Public Domain", provenance, StringComparison.Ordinal);
        Assert.Contains("13d0103bab5c5e7783a38af0712696fdc361a850eff3cd68ef8c2a0767d31b46", provenance, StringComparison.Ordinal);
    }

    [Fact]
    public void ClientStagingScriptContainsTheRequiredFailClosedChecks()
    {
        string script = File.ReadAllText(Path.Combine(
            RepositoryRoot,
            "scripts",
            "cook-character-presentation-for-client.sh"));

        Assert.Contains(".pm/project_id.txt", script, StringComparison.Ordinal);
        Assert.Contains(".pm/linked_projects.yaml", script, StringComparison.Ordinal);
        Assert.Contains("ls-files --stage --", script, StringComparison.Ordinal);
        Assert.Contains("check-ignore -q", script, StringComparison.Ordinal);
        Assert.Contains("ls-files -- \"$relative_output\"", script, StringComparison.Ordinal);
        Assert.Contains("! -L", script, StringComparison.Ordinal);
        Assert.DoesNotContain("rm -rf", script, StringComparison.Ordinal);
    }

    [Fact]
    public void ClientStagingScriptRestoresBeforeItsNoRestoreBuildAndNoBuildRun()
    {
        string script = File.ReadAllText(Path.Combine(
            RepositoryRoot,
            "scripts",
            "cook-character-presentation-for-client.sh"));

        int restore = script.IndexOf("dotnet restore", StringComparison.Ordinal);
        int build = script.IndexOf("dotnet build", StringComparison.Ordinal);
        int run = script.IndexOf("dotnet run", StringComparison.Ordinal);

        Assert.True(restore >= 0, "The staging workflow must restore the focused cooker project.");
        Assert.True(build > restore, "The staging workflow must build after restore.");
        Assert.True(run > build, "The staging workflow must run after build.");

        string buildCommand = script[build..run];
        string runCommand = script[run..];
        Assert.Contains("--no-restore", buildCommand, StringComparison.Ordinal);
        Assert.Contains("--no-restore", runCommand, StringComparison.Ordinal);
        Assert.Contains("--no-build", runCommand, StringComparison.Ordinal);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "ChronoFall.slnx")))
                return directory.FullName;
            directory = directory.Parent;
        }
        throw new DirectoryNotFoundException("Could not locate the ChronoFall repository root.");
    }
}
