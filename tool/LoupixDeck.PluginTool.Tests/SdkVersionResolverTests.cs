using System.Net;
using LoupixDeck.PluginTool;

namespace LoupixDeck.PluginTool.Tests;

public sealed class SdkVersionResolverTests
{
    [Fact]
    public void PickLatestStable_SkipsPrereleasesAndComparesNumerically()
    {
        // Arrange
        const string index = """{ "versions": ["1.9.0", "1.31.0", "1.4.2", "2.0.0-beta.1"] }""";

        // Act
        Version? latest = SdkVersionResolver.PickLatestStable(index);

        // Assert
        Assert.Equal(new Version(1, 31, 0), latest);
    }

    [Fact]
    public void PickLatestStable_NoStableVersion_ReturnsNull()
    {
        // Act
        Version? latest = SdkVersionResolver.PickLatestStable("""{ "versions": ["1.0.0-rc.1"] }""");

        // Assert
        Assert.Null(latest);
    }

    [Fact]
    public async Task ResolveAsync_FeedUnreachable_FallsBackToEmbeddedVersion()
    {
        // Arrange
        using HttpClient http = new(new FailingHandler());

        // Act
        (Version version, string source) = await SdkVersionResolver.ResolveAsync(http);

        // Assert
        Assert.Equal(SdkVersionResolver.Embedded, version);
        Assert.Contains("not reachable", source);
    }

    private sealed class FailingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            throw new HttpRequestException("offline", null, HttpStatusCode.ServiceUnavailable);
    }
}
