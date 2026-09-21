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
/// On devices with side strips the segmented strip shows
/// <see cref="GetValueText"/> as the dial indicator.
/// </summary>
public interface IAdjustmentCommand : IPluginCommand
{
    /// <summary>Applies a relative change of <paramref name="ticks"/> (positive
    /// for right-turn, negative for left-turn).</summary>
    Task ApplyAdjustment(CommandContext ctx, int ticks);

    /// <summary>Resets the value (typically invoked by an encoder press).</summary>
    Task ApplyReset(CommandContext ctx);

    /// <summary>Current value as display text (e.g. "75%"), or null for no
    /// overlay. Called by the host when rendering a dial indicator.</summary>
    string? GetValueText(CommandContext ctx);
}
