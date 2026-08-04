using System.Xml.Linq;

namespace ChronoFall.Box3D.Tests;

public sealed class ArchitectureBoundaryTests
{
    private static readonly string Root = FindRoot();

    [Fact]
    public void SharedBox3DProjectsRemainChildIndependentAndHeadless()
    {
        string[] projects =
        [
            Path.Combine(Root, "src", "ChronoFall.Box3D.Bindings", "ChronoFall.Box3D.Bindings.csproj"),
            Path.Combine(Root, "src", "ChronoFall.Box3D", "ChronoFall.Box3D.csproj")
        ];
        foreach (string project in projects)
        {
            string text = File.ReadAllText(project);
            Assert.DoesNotContain("Royale", text, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Starfall", text, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("SDL", text, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("ImGui", text, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Gpu", text, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void FamilyConsumerDirectlyReferencesOnlyManagedBox3DBoundary()
    {
        XDocument document = XDocument.Load(Path.Combine(Root, "tests", "ChronoFall.Box3D.FamilySourceConsumer", "ChronoFall.Box3D.FamilySourceConsumer.csproj"));
        string[] references = document.Descendants("ProjectReference").Select(x => (string)x.Attribute("Include")!).ToArray();
        Assert.Equal(["$(ChronoFallFamilyRoot)src/ChronoFall.Box3D/ChronoFall.Box3D.csproj"], references);
    }

    private static string FindRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "ChronoFall.slnx"))) directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("ChronoFall root not found.");
    }
}
