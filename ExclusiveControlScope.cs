namespace LoupixDeck.PluginSdk;

/// <summary>
/// The device controls an <see cref="IExclusiveModeProvider"/> overrides while it is
/// active. Every flag covers a control's input <em>and</em> its rendering; controls
/// outside the declared scope keep their normal page assignments and stay fully usable
/// (commands, plugin side strips, page swipes).
/// </summary>
/// <remarks>
/// The default for a provider is <see cref="All"/>, which is the whole-device takeover
/// that exclusive mode always performed — declaring nothing keeps the previous behaviour.
/// Supported devices have no separate "main display" surface: the main display area
/// <em>is</em> the touch-button grid, so it is covered by <see cref="TouchButtons"/>.
/// </remarks>
[Flags]
public enum ExclusiveControlScope
{
    /// <summary>Nothing is overridden.</summary>
    None = 0,

    /// <summary>The centre touch-button grid — taps and slot rendering (the main display area).</summary>
    TouchButtons = 1 << 0,

    /// <summary>The left/right side displays — strip rendering plus strip taps and swipes.
    /// No effect on devices without side strips.</summary>
    SideDisplays = 1 << 1,

    /// <summary>Turning a rotary encoder.</summary>
    RotaryTurn = 1 << 2,

    /// <summary>Pressing a rotary encoder.</summary>
    RotaryPress = 1 << 3,

    /// <summary>The hardware (LED) buttons.</summary>
    SimpleButtons = 1 << 4,

    /// <summary>Every control — a whole-device takeover. The default.</summary>
    All = TouchButtons | SideDisplays | RotaryTurn | RotaryPress | SimpleButtons
}
