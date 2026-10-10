using LoupixDeck.PluginTool;

namespace LoupixDeck.PluginTool.Tests;

public sealed class PluginNameTests
{
    [Fact]
    public void Parse_PascalCaseName_DerivesAllNames()
    {
        // Act
        PluginName name = PluginName.Parse("SpotifyPremium");

        // Assert
        Assert.Equal("LoupixDeck.Plugin.SpotifyPremium", name.ProjectName);
        Assert.Equal("SpotifyPremiumPlugin", name.ClassName);
        Assert.Equal("spotifypremium", name.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("spotify")]
    [InlineData("Spotify Premium")]
    [InlineData("Spotify.Premium")]
    [InlineData("1Password")]
    public void Parse_InvalidName_Throws(string? input)
    {
        // Act / Assert
        Assert.Throws<ToolException>(() => PluginName.Parse(input));
    }
}
