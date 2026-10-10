using System.Text.RegularExpressions;

namespace LoupixDeck.PluginTool;

/// <summary>The names derived from the PascalCase plugin name given to <c>loupix new</c>.</summary>
internal sealed partial record PluginName
{
    private PluginName(string name) => Name = name;

    /// <summary>The name as given, e.g. <c>SpotifyPremium</c>; also the display name.</summary>
    public string Name { get; }

    /// <summary>Project, assembly, folder and namespace name: <c>LoupixDeck.Plugin.&lt;Name&gt;</c>.</summary>
    public string ProjectName => $"LoupixDeck.Plugin.{Name}";

    public string ClassName => $"{Name}Plugin";

    /// <summary>Plugin id: the name in lowercase. Also the plugin folder name on the host.</summary>
    public string Id => Name.ToLowerInvariant();

    public static PluginName Parse(string? name)
    {
        if (string.IsNullOrEmpty(name) || !NamePattern().IsMatch(name))
        {
            throw new ToolException(
                $"Invalid plugin name '{name}'. Use a single PascalCase word, e.g. 'loupix new SpotifyPremium'.");
        }

        return new PluginName(name);
    }

    [GeneratedRegex("^[A-Z][A-Za-z0-9]*$")]
    private static partial Regex NamePattern();
}
