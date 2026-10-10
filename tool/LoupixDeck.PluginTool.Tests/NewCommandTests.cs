using System.Text.Json;
using LoupixDeck.PluginTool;

namespace LoupixDeck.PluginTool.Tests;

public sealed class NewCommandTests
{
    private static readonly PluginName Name = PluginName.Parse("Demo");

    [Fact]
    public void Scaffold_FillsEveryPlaceholder()
    {
        // Act
        Dictionary<string, string> files = NewCommand.Scaffold(Name, "Linux", "Jane", new Version(1, 31, 0), "../feed");

        // Assert
        Assert.All(files, f => Assert.DoesNotContain("{{", f.Value));
    }

    [Fact]
    public void Scaffold_ManifestMatchesNamesAndSdkVersion()
    {
        // Act
        Dictionary<string, string> files = NewCommand.Scaffold(Name, "Linux", "Jane", new Version(1, 31, 0), null);

        // Assert
        using JsonDocument manifest = JsonDocument.Parse(files["plugin.json"]);
        JsonElement root = manifest.RootElement;
        Assert.Equal("demo", root.GetProperty("id").GetString());
        Assert.Equal("1.31", root.GetProperty("sdkVersion").GetString());
        Assert.Equal("LoupixDeck.Plugin.Demo.dll", root.GetProperty("entryAssembly").GetString());
        Assert.Equal("Linux", root.GetProperty("platform").GetString());
        Assert.Equal("Jane", root.GetProperty("author").GetString());
        Assert.Contains("""Include="LoupixDeck.PluginSdk" Version="1.31.0">""", files["LoupixDeck.Plugin.Demo.csproj"]);
        Assert.Contains("SdkVersion = new Version(1, 31, 0)", files["DemoPlugin.cs"]);
    }

    [Fact]
    public void Scaffold_WithoutLocalFeed_HasNoNugetConfig()
    {
        // Act
        Dictionary<string, string> files = NewCommand.Scaffold(Name, "All", "", new Version(1, 31, 0), null);

        // Assert
        Assert.DoesNotContain("nuget.config", files.Keys);
        Assert.Contains(".github/workflows/release.yml", files.Keys);
    }

    [Fact]
    public void Scaffold_WithLocalFeed_WritesNugetConfig()
    {
        // Act
        Dictionary<string, string> files = NewCommand.Scaffold(Name, "All", "", new Version(1, 31, 0), "../sdk/nupkg");

        // Assert
        Assert.Contains("""value="../sdk/nupkg" """, files["nuget.config"]);
    }
}
