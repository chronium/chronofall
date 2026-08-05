namespace ChronoFall.EditorUi.SdlGpu.Tests;

public sealed class OptionsTests
{
    [Fact]
    public void NullIniPathDisablesPersistenceByDefault()
    {
        SdlGpuImGuiBackendOptions options = default;

        options.Validate();
        Assert.Null(options.IniPath);
        Assert.False(options.EnableDocking);
        Assert.Null(options.ConfigureFonts);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void BlankIniPathIsRejected(string path)
    {
        var options = new SdlGpuImGuiBackendOptions(IniPath: path);

        Assert.Throws<ArgumentException>(options.Validate);
    }
}
