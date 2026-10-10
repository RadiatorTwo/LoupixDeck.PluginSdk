using System.IO.Compression;
using LoupixDeck.PluginTool;

namespace LoupixDeck.PluginTool.Tests;

public sealed class InstallCommandTests : IDisposable
{
    private readonly DirectoryInfo _root = Directory.CreateTempSubdirectory("loupix-test-");

    public void Dispose() => _root.Delete(recursive: true);

    [Theory]
    [InlineData(false, ".config/LoupixDeck/plugins")]
    [InlineData(true, ".config/LoupixDeck/debug/plugins")]
    public void PluginsRoot_MatchesHostConfigDir(bool debug, string expected)
    {
        // Act
        string root = InstallCommand.PluginsRoot(debug, "/home/user");

        // Assert
        Assert.Equal(Path.Combine("/home/user", expected), root);
    }

    [Fact]
    public void Install_ReplacesPackageFilesAndKeepsUserFiles()
    {
        // Arrange
        string target = Path.Combine(_root.FullName, "demo");
        Directory.CreateDirectory(target);
        File.WriteAllText(Path.Combine(target, "settings.json"), "user settings");
        File.WriteAllText(Path.Combine(target, "Old.dll"), "stale");

        string content = Path.Combine(_root.FullName, "content");
        Directory.CreateDirectory(content);
        File.WriteAllText(Path.Combine(content, "plugin.json"), "{}");
        string package = Path.Combine(_root.FullName, "demo.zip");
        ZipFile.CreateFromDirectory(content, package);

        // Act
        InstallCommand.Install(package, target);

        // Assert
        Assert.True(File.Exists(Path.Combine(target, "plugin.json")));
        Assert.False(File.Exists(Path.Combine(target, "Old.dll")));
        Assert.Equal("user settings", File.ReadAllText(Path.Combine(target, "settings.json")));
    }
}
