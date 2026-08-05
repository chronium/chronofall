using System.Xml.Linq;

namespace ChronoFall.EditorUi.SdlGpu.Tests;

public sealed class ArchitectureBoundaryTests
{
    private static string RepositoryRoot { get; } = FindRepositoryRoot();

    [Fact]
    public void SharedProjectReferencesOnlyPinnedUiImplementationDependencies()
    {
        XDocument project = XDocument.Load(Path.Combine(
            RepositoryRoot,
            "src",
            "ChronoFall.EditorUi.SdlGpu",
            "ChronoFall.EditorUi.SdlGpu.csproj"));
        string[] references = project.Descendants("ProjectReference")
            .Select(reference => reference.Attribute("Include")?.Value)
            .Where(static value => value is not null)
            .Cast<string>()
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
        [
            "../../thirdparty/repos/ImGui.Net/Generator/Evergine.Bindings.Imgui/Evergine.Bindings.Imgui.csproj",
            "../../thirdparty/repos/SDL3-CS/SDL3-CS/SDL3-CS.csproj",
        ],
        references);
        Assert.Empty(project.Descendants("PackageReference"));
    }

    [Fact]
    public void NativeBuildContainsOnlyTheApprovedSurface()
    {
        string script = File.ReadAllText(Path.Combine(RepositoryRoot, "thirdparty", "build-imgui-macos.sh"));

        Assert.Contains("imgui_impl_sdl3.cpp", script, StringComparison.Ordinal);
        Assert.Contains("imgui_impl_sdlgpu3.cpp", script, StringComparison.Ordinal);
        Assert.Contains("cimguizmo.cpp", script, StringComparison.Ordinal);
        Assert.Contains("ImGuizmo.cpp", script, StringComparison.Ordinal);
        Assert.DoesNotContain("imgui_demo.cpp", script, StringComparison.Ordinal);
        Assert.DoesNotContain("cimplot", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("cimnodes", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("GraphEditor", script, StringComparison.Ordinal);
        Assert.DoesNotContain("ImSequencer", script, StringComparison.Ordinal);
        Assert.DoesNotContain("pkg-config", script, StringComparison.Ordinal);
        Assert.Contains("SDL3-CS/External/SDL", script, StringComparison.Ordinal);
    }

    [Fact]
    public void BindingPatchRemovesExcludedManagedAndPrebuiltRuntimeSurfaces()
    {
        string patch = File.ReadAllText(Path.Combine(
            RepositoryRoot,
            "thirdparty",
            "patches",
            "ImGui.Net",
            "0001-limit-bindings-and-runtime-surface.patch"));

        Assert.Contains("Compile Remove=\"Implot/**/*.cs\"", patch, StringComparison.Ordinal);
        Assert.Contains("Compile Remove=\"Imnodes/**/*.cs\"", patch, StringComparison.Ordinal);
        Assert.Contains("Content Include=\"runtimes", patch, StringComparison.Ordinal);
        Assert.Contains("System.Runtime.CompilerServices.Unsafe", patch, StringComparison.Ordinal);
    }

    [Fact]
    public void DependencyPinsAndLicenceEvidenceAreCommitted()
    {
        string versions = File.ReadAllText(Path.Combine(RepositoryRoot, "thirdparty", "versions.env"));
        Assert.Contains("IMGUI_NET_COMMIT=\"1f97beecfc9b83e1549e9782757cf85b1777cb9d\"", versions, StringComparison.Ordinal);
        Assert.Contains("CIMGUI_COMMIT=\"715802490eabca2fc86cf25b41b83aa7c5d6060d\"", versions, StringComparison.Ordinal);
        Assert.Contains("IMGUI_COMMIT=\"2a1b69f05748ad909f03acf4533447cac1331611\"", versions, StringComparison.Ordinal);
        Assert.Contains("CIMGUIZMO_COMMIT=\"77e8ff47dc16a688edb06526b2f19c845b653bc7\"", versions, StringComparison.Ordinal);
        Assert.Contains("IMGUIZMO_COMMIT=\"b10e91756d32395f5c1fefd417899b657ed7cb88\"", versions, StringComparison.Ordinal);
        Assert.Contains("SDL_SOURCE_COMMIT=\"f0e99e7c7f9aa90d5ce2e3b8a69f72c23faf257e\"", versions, StringComparison.Ordinal);

        string[] evidence =
        [
            "thirdparty/licenses/ImGui.Net/ImGui.Net-LICENSE",
            "thirdparty/licenses/ImGui.Net/cimgui-LICENSE",
            "thirdparty/licenses/ImGui.Net/Dear-ImGui-LICENSE.txt",
            "thirdparty/licenses/ImGui.Net/cimguizmo-LICENSE",
            "thirdparty/licenses/ImGui.Net/ImGuizmo-LICENSE",
            "thirdparty/licenses/Evergine.Mathematics/LICENSE.txt",
            "thirdparty/licenses/Evergine.Mathematics/PROVENANCE.md",
        ];
        Assert.All(evidence, path => Assert.True(File.Exists(Path.Combine(RepositoryRoot, path)), path));

        string provenance = File.ReadAllText(Path.Combine(
            RepositoryRoot,
            "thirdparty",
            "licenses",
            "Evergine.Mathematics",
            "PROVENANCE.md"));
        Assert.Contains("d417512a72fef6239c6736b5efee06ca4b54cd3b453e5c23ab3902035979d499", provenance, StringComparison.Ordinal);
        Assert.Contains("aec51fd78e190ce5d476e07dbaa1acc123cb0b80adc10bfad6ffb513ad296cb9", provenance, StringComparison.Ordinal);
    }

    [Fact]
    public void SmokeConsumerUsesOnlyTheEditorUiFamilySourceReference()
    {
        XDocument project = XDocument.Load(Path.Combine(
            RepositoryRoot,
            "tests",
            "ChronoFall.EditorUi.FamilySourceConsumer",
            "ChronoFall.EditorUi.FamilySourceConsumer.csproj"));
        XElement reference = Assert.Single(project.Descendants("ProjectReference"));

        Assert.Equal(
            "$(ChronoFallFamilyRoot)src/ChronoFall.EditorUi.SdlGpu/ChronoFall.EditorUi.SdlGpu.csproj",
            reference.Attribute("Include")?.Value);
        Assert.Empty(project.Descendants("PackageReference"));
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
