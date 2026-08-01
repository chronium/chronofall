namespace ChronoFall.CharacterPresentation.Tests;

public sealed class AssemblyBoundaryTests
{
    [Fact]
    public void CorePresentationAssemblyHasOnlyFrameworkReferences()
    {
        string[] references = typeof(SkeletonDefinition).Assembly
            .GetReferencedAssemblies()
            .Select(static assembly => assembly.Name!)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.DoesNotContain(references, name =>
            name.Contains("SDL", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("SimpleMesh", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Royale", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Starfall", StringComparison.OrdinalIgnoreCase));
    }
}
