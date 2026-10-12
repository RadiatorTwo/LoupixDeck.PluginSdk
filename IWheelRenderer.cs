namespace LoupixDeck.PluginSdk;

/// <summary>
/// What the host knows about the wheel screen when it asks an <see cref="IWheelRenderer"/>
/// to draw it.
/// </summary>
/// <param name="Command">
/// The context of the command bound to the wheel: its parameters, <see cref="ButtonTargets.RotaryEncoder"/>
/// as the target, the wheel's rotary index as <see cref="CommandContext.SourceIndex"/>, the device and
/// the host.
/// </param>
/// <param name="Size">
/// Side of the square wheel screen in device pixels, equal to the canvas'
/// <see cref="IRenderCanvas.Width"/> and <see cref="IRenderCanvas.Height"/>. Only the inscribed circle
/// is visible on the round glass. Derive every coordinate from this value instead of assuming 240.
/// </param>
/// <param name="Label">The label the user gave this wheel mode, or <c>null</c> when it has none.</param>
/// <param name="Value">
/// The current value of the wheel's adjustment command, as the host would draw it in the normal
/// wheel look, or <c>null</c> when no adjustment command reports one.
/// </param>
public sealed record WheelRenderContext(CommandContext Command, int Size, string? Label, AdjustmentValue? Value);

/// <summary>
/// Optional interface for a command that draws the Loupedeck CT's round wheel screen itself.
/// Implement it on a command that is bound to the wheel (as its press, left or right command),
/// typically next to <see cref="IAdjustmentCommand"/>, the same way <see cref="IDisplayImageCommand"/>
/// lets a command draw its touch button.
/// </summary>
/// <remarks>
/// <para>The host asks the renderer first on every wheel repaint. When it returns <c>false</c> or
/// throws, the host draws the wheel as it would without the renderer: the user's wheel canvas, or
/// the automatic label and value arc. On a device without a wheel screen the renderer is never called.</para>
/// <para>Added in SDK 1.32.0. An older host does not know the interface and never calls it, so a
/// plugin that must also run there keeps reporting <see cref="IAdjustmentCommand.GetValue"/> and
/// checks <see cref="SdkInfo.Version"/> before relying on the custom look.</para>
/// </remarks>
public interface IWheelRenderer
{
    /// <summary>
    /// How often the host repaints the wheel while this command is bound to the active wheel mode.
    /// <see cref="TimeSpan.Zero"/> repaints only on demand: when the page or wheel mode changes, when
    /// the wheel is turned, and on <see cref="IPluginHost.RequestButtonRefresh"/>. The host caps the
    /// rate at 30 repaints per second.
    /// </summary>
    TimeSpan Interval { get; }

    /// <summary>
    /// Draws the wheel screen. The canvas is cleared to black before the call and is valid only
    /// during it. Keep the call fast and free of blocking work; fetch data elsewhere and draw the
    /// latest snapshot here.
    /// </summary>
    /// <returns><c>true</c> when the canvas holds the frame to show; <c>false</c> for the normal wheel look.</returns>
    bool RenderWheel(IRenderCanvas canvas, WheelRenderContext context);
}
