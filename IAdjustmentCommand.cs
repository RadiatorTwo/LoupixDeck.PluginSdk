namespace LoupixDeck.PluginSdk;

/// <summary>
/// A plugin command that represents a rotary-encoder value adjustment: a
/// continuous parameter the user dials up/down (volume, brightness, zoom),
/// with a press to reset or commit. Set
/// <see cref="IPluginCommand.SupportedTargets"/> to
/// <see cref="ButtonTargets.RotaryEncoder"/>.
///
/// The host dispatches this interface from the rotary encoders: a turn runs
/// <see cref="ApplyAdjustment"/> with the encoder's tick delta, a knob press runs
/// <see cref="ApplyReset"/>. Binding the command to a single turn slot is enough —
/// an empty opposite turn slot and an empty press slot borrow it. Any other target
/// (touch button, simple button, macro, CLI) still calls
/// <see cref="IPluginCommand.Execute"/>, so implement that as the non-dial fallback.
/// On devices with side strips the dial shows <see cref="GetValue"/> as an indicator: the host
/// draws a generic bar with its text, and a side-strip provider that renders the segment itself
/// can pull the same value through <see cref="SideStripRotary.GetValue"/>.
/// </summary>
public interface IAdjustmentCommand : IPluginCommand
{
    /// <summary>Applies a relative change of <paramref name="ticks"/> (positive
    /// for right-turn, negative for left-turn).</summary>
    Task ApplyAdjustment(CommandContext ctx, int ticks);

    /// <summary>Resets the value (typically invoked by an encoder press).</summary>
    Task ApplyReset(CommandContext ctx);

    /// <summary>
    /// Current value for the dial indicator — a position on the command's scale plus its
    /// display text — or null for no indicator. Called by the host on the render path and by
    /// a side-strip provider drawing this dial, so it must be fast, synchronous and free of
    /// side effects. Returns null by default, which leaves the dial showing its static label.
    /// </summary>
    AdjustmentValue? GetValue(CommandContext ctx) => null;

    /// <summary>
    /// Current value as display text (e.g. "75%"), or null for no caption. Defaults to the
    /// text of <see cref="GetValue"/>, so a command that implements the richer member gets
    /// this one for free; override it to supply text without a scale position.
    /// </summary>
    string? GetValueText(CommandContext ctx) => GetValue(ctx)?.Text;
}
