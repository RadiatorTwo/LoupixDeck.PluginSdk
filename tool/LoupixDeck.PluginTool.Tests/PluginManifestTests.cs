using LoupixDeck.PluginTool;

namespace LoupixDeck.PluginTool.Tests;

public sealed class PluginManifestTests
{
    [Theory]
    [InlineData("All", "any")]
    [InlineData("", "any")]
    [InlineData("windows", "windows")]
    [InlineData("Linux", "linux")]
    [InlineData("macOS", "macos")]
    public void Parse_Platform_MapsToPackageSuffix(string platform, string suffix)
    {
        // Arrange
        string json = $$"""{ "id": "demo", "version": "1.2.3", "sdkVersion": "1.31", "entryAssembly": "Demo.dll", "platform": "{{platform}}" }""";

        // Act
        PluginManifest manifest = PluginManifest.Parse(json);

        // Assert
        Assert.Equal($"demo-1.2.3-{suffix}.zip", manifest.PackageFileName);
    }

    [Fact]
    public void Parse_MissingPlatform_IsAny()
    {
        // Act
        PluginManifest manifest = PluginManifest.Parse("""{ "id": "demo", "version": "1.0.0", "sdkVersion": "1.31", "entryAssembly": "Demo.dll" }""");

        // Assert
        Assert.Equal("any", manifest.PlatformSuffix);
    }

    [Fact]
    public void Parse_InvalidManifest_ReportsEveryProblem()
    {
        // Arrange
        const string json = """{ "id": "../x", "version": "1.0", "sdkVersion": "abc", "platform": "Amiga" }""";

        // Act
        ToolException ex = Assert.Throws<ToolException>(() => PluginManifest.Parse(json));

        // Assert
        Assert.Contains("'id'", ex.Message);
        Assert.Contains("'entryAssembly'", ex.Message);
        Assert.Contains("'version'", ex.Message);
        Assert.Contains("'sdkVersion'", ex.Message);
        Assert.Contains("'platform'", ex.Message);
    }

    [Fact]
    public void Parse_NotJson_Throws()
    {
        // Act / Assert
        Assert.Throws<ToolException>(() => PluginManifest.Parse("{ nope"));
    }
}
