namespace LoupixDeck.PluginSdk;

/// <summary>
/// High-level section a command group is filed under in the command picker.
/// </summary>
public enum CommandGroupSection
{
    /// <summary>Built-in device / navigation commands.</summary>
    Core,

    /// <summary>Built-in and user macros.</summary>
    Macros,

    /// <summary>Commands contributed by plugins.</summary>
    Plugins
}

/// <summary>
/// Declarative metadata for a command group (category) shown as a card in the
/// command picker. A plugin returns these from
/// <see cref="LoupixPlugin.GetCommandGroups"/> so its categories get a proper
/// icon, description and section instead of the generic fallback. Entirely
/// optional and cosmetic — a group without a descriptor still works.
/// </summary>
public sealed class CommandGroupDescriptor
{
    /// <summary>The group name, matching the <see cref="CommandDescriptor.Group"/>
    /// of the commands it describes.</summary>
    public required string Group { get; init; }

    /// <summary>Short description shown under the category title on the card.</summary>
    public string? Description { get; init; }

    /// <summary>Material Design Icons glyph (a single code point) shown on the card.</summary>
    public string? Icon { get; init; }

    /// <summary>Section the category is filed under. Defaults to
    /// <see cref="CommandGroupSection.Plugins"/>.</summary>
    public CommandGroupSection Section { get; init; } = CommandGroupSection.Plugins;
}
