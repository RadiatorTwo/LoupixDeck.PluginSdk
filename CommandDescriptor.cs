namespace LoupixDeck.PluginSdk;

/// <summary>A single positional command parameter: its name and CLR type.</summary>
public sealed class CommandParameter(string name, Type parameterType)
{
    public string Name { get; } = name;
    public Type ParameterType { get; } = parameterType;

    /// <summary>
    /// Optional command-defined default value for this parameter. When set, the host
    /// pre-fills the command's settings editor with it as the command is inserted into a
    /// sequence, so an adjustment command (e.g. a volume step) comes up with sensible,
    /// command-specific defaults instead of a blank/type default. Null means no default.
    /// Additive since SDK 1.17.0 — plugins built against an earlier SDK simply leave it null.
    /// </summary>
    public string? DefaultValue { get; init; }
}

/// <summary>
/// Declarative description of a plugin command. Replaces the core's
/// <c>[Command]</c> attribute. The <see cref="CommandName"/> is the stable
/// identifier persisted in button assignments — it MUST never change once a
/// plugin has shipped, otherwise existing <c>config.json</c> files break.
/// </summary>
public sealed class CommandDescriptor
{
    /// <summary>Stable command identifier, e.g. <c>System.ObsStartRecord</c>.</summary>
    public required string CommandName { get; init; }

    /// <summary>Label shown in the command-selection menu.</summary>
    public required string DisplayName { get; init; }

    /// <summary>Group/category the command is listed under in the menu.</summary>
    public required string Group { get; init; }

    /// <summary>
    /// Optional Material Design Icons glyph (a single code point, e.g. <c>"A"</c>)
    /// shown next to the command in the command picker. Null falls back to the
    /// category's icon. Purely cosmetic — never persisted.
    /// </summary>
    public string? Icon { get; init; }

    /// <summary>
    /// Optional one-line description shown as the command's subtitle in the picker.
    /// Null shows no subtitle. Purely cosmetic — never persisted.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>Parameter placeholder template, e.g. <c>({SceneName})</c>.
    /// Null when the command takes no parameters.</summary>
    public string? ParameterTemplate { get; init; }

    /// <summary>Positional parameter definitions, in declaration order.</summary>
    public IReadOnlyList<CommandParameter> Parameters { get; init; } = [];

    /// <summary>
    /// The button states this command brings along, in order. When non-empty the host
    /// materializes exactly these states on a button the command is inserted into, locks
    /// state management in the editor and reports the rendered state through
    /// <see cref="CommandContext.StateName"/>; the plugin drives the active state via
    /// <see cref="IPluginHost.SetActiveButtonState"/>. Empty (the default) keeps the
    /// pre-1.21.0 behaviour: states stay entirely user-managed.
    /// </summary>
    public IReadOnlyList<ButtonStateDescriptor> States { get; init; } = [];

    /// <summary>
    /// When true the command is not listed as a plain leaf in the command
    /// selection menu — it is instead surfaced through a dynamic submenu the
    /// plugin builds via <see cref="IMenuContributor"/> (e.g. one entry per OBS
    /// scene). The command stays fully registered and executable.
    /// </summary>
    public bool HiddenFromMenu { get; init; }
}
