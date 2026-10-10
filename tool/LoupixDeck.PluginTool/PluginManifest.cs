using System.Text.Json;
using System.Text.RegularExpressions;

namespace LoupixDeck.PluginTool;

/// <summary>
/// The parts of <c>plugin.json</c> packing needs, checked with the rules the host
/// (PluginManager / PluginInstaller) and the plugin-release.yml workflow apply.
/// </summary>
internal sealed partial record PluginManifest(string Id, string Version, string EntryAssembly, string PlatformSuffix)
{
    /// <summary>Store package name, identical to the one plugin-release.yml produces.</summary>
    public string PackageFileName => $"{Id}-{Version}-{PlatformSuffix}.zip";

    public static PluginManifest Load(string path)
    {
        if (!File.Exists(path))
            throw new ToolException($"No plugin.json at '{path}'.");
        return Parse(File.ReadAllText(path));
    }

    internal static PluginManifest Parse(string json)
    {
        JsonElement root;
        try
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            root = doc.RootElement.Clone();
        }
        catch (JsonException ex)
        {
            throw new ToolException($"plugin.json is not valid JSON: {ex.Message}");
        }

        List<string> problems = [];
        string id = Read(root, "id");
        string version = Read(root, "version");
        string sdkVersion = Read(root, "sdkVersion");
        string entry = Read(root, "entryAssembly");
        string platform = Read(root, "platform");

        if (id.Length == 0)
            problems.Add("'id' is missing.");
        else if (!IsSafeFolderName(id))
            problems.Add($"'id' \"{id}\" is not a valid folder name.");
        if (entry.Length == 0)
            problems.Add("'entryAssembly' is missing.");
        if (!VersionPattern().IsMatch(version))
            problems.Add($"'version' \"{version}\" is not major.minor.patch.");
        if (!System.Version.TryParse(sdkVersion, out _))
            problems.Add($"'sdkVersion' \"{sdkVersion}\" is not a version.");

        string? suffix = platform.ToLowerInvariant() switch
        {
            "windows" => "windows",
            "linux" => "linux",
            "macos" => "macos",
            "all" or "" => "any",
            _ => null
        };
        if (suffix is null)
            problems.Add($"'platform' \"{platform}\" is not All, Windows, Linux or macOS.");

        if (problems.Count > 0)
            throw new ToolException("plugin.json is invalid:" + string.Concat(problems.Select(p => $"{Environment.NewLine}  - {p}")));

        return new PluginManifest(id, version, entry, suffix!);
    }

    private static string Read(JsonElement root, string name) =>
        root.TryGetProperty(name, out JsonElement value) && (value.ValueKind == JsonValueKind.String)
            ? value.GetString()!.Trim()
            : "";

    // Same rule as PluginInstaller.IsSafeFolderName: one path segment, no traversal.
    private static bool IsSafeFolderName(string name) =>
        name is not ("." or "..")
        && (name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0)
        && (name.IndexOfAny(['/', '\\']) < 0);

    [GeneratedRegex(@"^[0-9]+\.[0-9]+\.[0-9]+$")]
    private static partial Regex VersionPattern();
}
