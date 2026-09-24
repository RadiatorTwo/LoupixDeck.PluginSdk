namespace LoupixDeck.PluginSdk;

/// <summary>
/// Everything a command needs at execution time. Wraps the positional
/// parameters parsed from the persisted <c>CommandName(arg1,arg2)</c> string,
/// the button type that triggered the command and the host bridge.
/// </summary>
public sealed class CommandContext
{
    /// <summary>Positional parameters, exactly as parsed from the command
    /// string's parentheses. Never null; empty when the command takes none.</summary>
    public required string[] Parameters { get; init; }

    /// <summary>The button type the command was invoked from.</summary>
    public required ButtonTargets Target { get; init; }

    /// <summary>
    /// Identifier of the originating control (rotary index, touch slot, simple
    /// button index) when <see cref="Target"/> denotes an indexed source.
    /// Null when invoked from chained commands or CLI.
    /// </summary>
    public int? SourceIndex { get; init; }

    /// <summary>The active device, or null if none.</summary>
    public DeviceInfo? Device { get; init; }

    /// <summary>The host bridge of the owning plugin.</summary>
    public required IPluginHost Host { get; init; }

    /// <summary>
    /// Name of the button state currently being rendered, for a command that declares its states
    /// through <see cref="CommandDescriptor.States"/>. Set on the display/render path only; null
    /// on execution, on stateless buttons and for commands that declare no states.
    /// Additive since SDK 1.21.0.
    /// </summary>
    public string? StateName { get; init; }

    /// <summary>
    /// All commands of the button's sequence, in order (this rendering command first).
    /// Empty for single-command buttons. Lets a display command compose a layout from
    /// its siblings (e.g. one row/bar per command). Only the first command of a sequence
    /// renders, so this is populated on the display/render path only.
    /// </summary>
    public IReadOnlyList<SequenceCommand> SequenceCommands { get; init; } = [];

    /// <summary>
    /// Opaque identity of the button the command is bound to. The host hands the same value to
    /// the render path (<see cref="IDisplayCommand"/>, <see cref="IDisplayImageCommand"/>,
    /// <see cref="IAnimatedDisplayCommand"/>) and to <see cref="IPluginCommand.Execute"/> when that
    /// button is pressed, so a plugin can keep per-button state — e.g. which page a paging tile
    /// shows — and advance exactly the button that was pressed. Two buttons never share a key,
    /// even when they carry identical commands and parameters.
    /// <para>The key is a runtime handle, not a persisted id: it stays stable while the button
    /// exists in this session and changes after a restart or a configuration reload. Do not store
    /// it. Null when the command runs without a button (rotary, CLI, chained from elsewhere) and
    /// on hosts older than SDK 1.26.0 — fall back to a parameter-derived key there.</para>
    /// Additive since SDK 1.26.0.
    /// </summary>
    public string? ButtonKey { get; init; }
}
