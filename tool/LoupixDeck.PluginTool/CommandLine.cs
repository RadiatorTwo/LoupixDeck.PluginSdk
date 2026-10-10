namespace LoupixDeck.PluginTool;

/// <summary>
/// Minimal argument parser: positional arguments plus <c>--name value</c> options and
/// <c>--flag</c> switches. Each command declares which options and switches it accepts.
/// </summary>
internal sealed class CommandLine
{
    private readonly Dictionary<string, string> _options = new(StringComparer.Ordinal);
    private readonly HashSet<string> _switches = new(StringComparer.Ordinal);

    public List<string> Positional { get; } = [];

    /// <param name="args">Arguments after the command name.</param>
    /// <param name="options">Accepted value options, e.g. <c>--output</c>.</param>
    /// <param name="switches">Accepted switches without a value, e.g. <c>--debug</c>.</param>
    /// <param name="aliases">Short forms, e.g. <c>-o</c> → <c>--output</c>.</param>
    public static CommandLine Parse(
        IReadOnlyList<string> args,
        IReadOnlyCollection<string> options,
        IReadOnlyCollection<string> switches,
        IReadOnlyDictionary<string, string>? aliases = null)
    {
        CommandLine result = new();
        for (int i = 0; i < args.Count; i++)
        {
            string arg = args[i];
            if (!arg.StartsWith('-'))
            {
                result.Positional.Add(arg);
                continue;
            }

            string name = aliases is not null && aliases.TryGetValue(arg, out string? full) ? full : arg;
            if (switches.Contains(name))
            {
                result._switches.Add(name);
            }
            else if (options.Contains(name))
            {
                if ((i + 1) >= args.Count)
                    throw new ToolException($"Option '{arg}' needs a value.");
                result._options[name] = args[++i];
            }
            else
            {
                throw new ToolException($"Unknown option '{arg}'.");
            }
        }

        return result;
    }

    public string? Get(string name) => _options.GetValueOrDefault(name);

    public bool Has(string name) => _switches.Contains(name);
}
