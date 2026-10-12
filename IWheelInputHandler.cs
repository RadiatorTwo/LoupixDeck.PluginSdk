namespace LoupixDeck.PluginSdk;

/// <summary>Where a <see cref="WheelTouch"/> is in its lifetime.</summary>
public enum WheelTouchPhase
{
    /// <summary>The finger went down on the wheel screen.</summary>
    Start,

    /// <summary>The finger moved while it stayed down.</summary>
    Move,

    /// <summary>The finger lifted off the wheel screen.</summary>
    End
}

/// <summary>
/// One touch frame on the Loupedeck CT's round wheel screen.
/// </summary>
/// <param name="Phase">Whether the finger went down, moved or lifted.</param>
/// <param name="X">Horizontal position in canvas space, from 0 to the screen size minus 1 (the same
/// space <see cref="IWheelRenderer.RenderWheel"/> draws in).</param>
/// <param name="Y">Vertical position in canvas space, from 0 to the screen size minus 1.</param>
/// <param name="Id">The firmware's id for the finger. All frames of one touch carry the same id.</param>
public readonly record struct WheelTouch(WheelTouchPhase Phase, int X, int Y, int Id);

/// <summary>Direction of a swipe across the Loupedeck CT wheel screen.</summary>
public enum WheelSwipeDirection
{
    Left,
    Right,
    Up,
    Down
}

/// <summary>
/// Optional interface for a command that reacts to touches and swipes on the Loupedeck CT's round
/// wheel screen. Implement it on a command that is bound to the active wheel mode (as its press, left
/// or right command), typically next to <see cref="IWheelRenderer"/>.
/// </summary>
/// <remarks>
/// <para>The host offers wheel input to the handler before its own gestures: a short tap presses the
/// wheel, a horizontal swipe switches the workspace and a vertical swipe opens the wheel-mode menu.
/// Returning <c>true</c> from <see cref="OnWheelTouch"/> for any phase of a touch claims that touch: the
/// host then runs no tap, swipe or menu for it. For a touch nobody claimed, the host classifies the
/// gesture and calls <see cref="OnWheelSwiped"/> for a swipe; <c>true</c> skips the built-in action.
/// Returning <c>false</c> everywhere keeps the normal wheel behaviour.</para>
/// <para>Input is never delivered while the device is off, while a display takeover or the
/// screensaver consumes the touch as a wake gesture, while the wheel-mode menu is open, or while an
/// exclusive provider owns the wheel (that provider receives it instead). Both methods run on the
/// device input thread; keep them fast and free of blocking work.</para>
/// <para>Added in SDK 1.32.0. An older host does not know the interface and never calls it; check
/// <see cref="SdkInfo.Version"/> when the plugin depends on it.</para>
/// </remarks>
public interface IWheelInputHandler
{
    /// <summary>A touch frame on the wheel screen.</summary>
    /// <param name="touch">Phase, position and finger id.</param>
    /// <param name="context">The context of the command bound to the wheel, with
    /// <see cref="ButtonTargets.RotaryEncoder"/> as the target and the wheel's rotary index as
    /// <see cref="CommandContext.SourceIndex"/>.</param>
    /// <returns><c>true</c> to claim the touch, so the host runs no gesture for it.</returns>
    bool OnWheelTouch(WheelTouch touch, CommandContext context);

    /// <summary>The host recognised a swipe across the wheel screen in a touch nobody claimed.</summary>
    /// <param name="direction">The direction the finger moved in.</param>
    /// <param name="context">The context of the command bound to the wheel.</param>
    /// <returns><c>true</c> when handled; <c>false</c> lets the host switch the workspace (left and
    /// right) or open the wheel-mode menu (up and down).</returns>
    bool OnWheelSwiped(WheelSwipeDirection direction, CommandContext context);
}
