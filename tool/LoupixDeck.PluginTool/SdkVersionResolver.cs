using System.Reflection;
using System.Text.Json;

namespace LoupixDeck.PluginTool;

/// <summary>
/// Finds the LoupixDeck.PluginSdk version a new plugin should reference: the latest stable
/// version on nuget.org, or the version this tool was released with when nuget.org is unreachable.
/// </summary>
internal static class SdkVersionResolver
{
    private const string IndexUrl = "https://api.nuget.org/v3-flatcontainer/loupixdeck.pluginsdk/index.json";

    public static Version Embedded { get; } = Version.Parse(
        Assembly.GetExecutingAssembly()
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .Single(a => a.Key == "SdkVersion")
            .Value!);

    public static async Task<(Version Version, string Source)> ResolveAsync(HttpClient http)
    {
        try
        {
            string json = await http.GetStringAsync(IndexUrl);
            Version? latest = PickLatestStable(json);
            if (latest is not null)
                return (latest, "nuget.org");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            // Offline or feed not reachable: fall through to the embedded version.
        }

        return (Embedded, "bundled with this tool, nuget.org not reachable");
    }

    /// <summary>Highest stable version in a NuGet flat-container <c>index.json</c>; prereleases are skipped.</summary>
    internal static Version? PickLatestStable(string indexJson)
    {
        using JsonDocument doc = JsonDocument.Parse(indexJson);
        return doc.RootElement.GetProperty("versions")
            .EnumerateArray()
            .Select(v => v.GetString())
            .Where(v => v is not null && !v.Contains('-'))
            .Select(v => Version.TryParse(v, out Version? parsed) ? parsed : null)
            .Where(v => v is not null)
            .Max();
    }
}
