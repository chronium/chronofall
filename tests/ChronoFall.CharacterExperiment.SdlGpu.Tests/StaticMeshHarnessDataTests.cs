namespace ChronoFall.CharacterExperiment.SdlGpu.Tests;

public sealed class StaticMeshHarnessDataTests
{
    [Fact]
    public void DiagnosticMeshIsDeterministicAndContainsTwoCompleteBoxes()
    {
        StaticMeshDefinition first = SdlGpuStaticMeshHarness.CreateDiagnosticMesh();
        StaticMeshDefinition second = SdlGpuStaticMeshHarness.CreateDiagnosticMesh();

        Assert.Equal("static-two-section-diagnostic", first.Name);
        Assert.Equal(48, first.Vertices.Count);
        Assert.Equal(72, first.Indices.Count);
        Assert.Equal(["diagnostic-orange", "diagnostic-blue"], first.Sections.Select(static section => section.MaterialName));
        Assert.Equal(first.Vertices, second.Vertices);
        Assert.Equal(first.Indices, second.Indices);
    }
}
