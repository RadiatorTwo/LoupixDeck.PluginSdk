namespace LoupixDeck.PluginSdk;

/// <summary>
/// A ready-made dial configuration a plugin contributes: one command per rotary
/// action, offered to the user as a single entry in the host's dial-preset
/// surfaces (the rotary context menu and the action panel's preset tab).
/// </summary>
/// <remarks>
/// <para>
/// A preset is a sibling of <see cref="MenuNode.RotaryGroup"/>, not a
/// replacement for it: a rotary group is one node of the plugin's own command
/// tree, a preset is a reusable configuration listed next to the host's
/// built-in presets. A plugin may offer both.
/// </para>
/// <para>
/// Applying a preset copies its commands onto the dial once. The dial keeps no
/// reference to the preset afterwards, so changing or removing the preset never
/// rewrites a dial that was configured from it.
/// </para>
/// <para>
/// The descriptors are re-read every time a preset surface is built, so the set
/// may follow live state: a plugin that owns devices normally emits one preset
/// per device, and one that appears or disappears is picked up on its own.
/// </para>
/// <para>
/// The host validates every descriptor and silently drops the ones it cannot
/// offer — an invalid preset never prevents the plugin from loading. A preset is
/// dropped when <see cref="Id"/> or <see cref="Name"/> is blank, when two
/// descriptors of the same plugin share an <see cref="Id"/>, when
/// <see cref="Actions"/> names no usable command, or when one of the commands it
/// names is not registered or cannot be assigned to a rotary encoder.
/// </para>
/// <para>
/// Presets contributed this way are read-only in the host: the user can apply
/// one, but cannot rename, edit or delete it. Saving a configured dial as a new
/// preset of their own stays available.
/// </para>
/// </remarks>
public sealed class DialPresetDescriptor
{
    /// <summary>
    /// Identifies the preset within its plugin. The host derives the preset's
    /// stable identity from the plugin id and this value, so it must not change
    /// between releases and must not be reused for a different preset.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>Label shown in the preset lists.</summary>
    public required string Name { get; init; }

    /// <summary>
    /// Material Design Icons glyph for the preset's row and menu entry, as the
    /// literal character (e.g. <c>"\U000F057E"</c>). The host falls back to its
    /// generic preset glyph when this is null or empty.
    /// </summary>
    public string? Glyph { get; init; }

    /// <summary>
    /// The command bound to each rotary action. An action left out of the map
    /// keeps whatever the dial already had, so a preset need not fill all three.
    /// <see cref="MenuCommandRef"/> carries parameter values, which the host
    /// bakes into the command string exactly as it does for a menu leaf.
    /// </summary>
    public IReadOnlyDictionary<RotaryAction, MenuCommandRef> Actions { get; init; }
        = new Dictionary<RotaryAction, MenuCommandRef>();
}
