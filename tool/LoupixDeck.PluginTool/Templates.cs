using System.Reflection;
using System.Text.RegularExpressions;

namespace LoupixDeck.PluginTool;

/// <summary>Reads the embedded scaffolding templates and fills their <c>{{Key}}</c> placeholders.</summary>
internal static partial class Templates
{
    public static string Read(string name)
    {
        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Templates/{name}")
                              ?? throw new InvalidOperationException($"Template '{name}' is not embedded.");
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }

    public static string Render(string template, IReadOnlyDictionary<string, string> values)
    {
        return Placeholder().Replace(template, match =>
            values.TryGetValue(match.Groups[1].Value, out string? value)
                ? value
                : throw new InvalidOperationException($"No value for template placeholder '{match.Value}'."));
    }

    [GeneratedRegex(@"\{\{(\w+)\}\}")]
    private static partial Regex Placeholder();
}
