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
/// <para>
/// The Loupedeck CT wheel is rotary index 6. By default it follows <see cref="RotaryTurn"/> and
/// <see cref="RotaryPress"/> like the side dials, so <see cref="All"/> takes it too. A provider
/// that declares <see cref="Wheel"/> owns it regardless of the rotary flags; one that sets
/// <see cref="IExclusiveModeProvider.WheelFollowsRotaryScope"/> to <c>false</c> leaves it out
/// of the rotary flags, which then cover the side dials only.
/// </para>
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

    /// <summary>Turning a rotary encoder. Includes the Loupedeck CT wheel unless the provider
    /// opts out through <see cref="IExclusiveModeProvider.WheelFollowsRotaryScope"/>.</summary>
    RotaryTurn = 1 << 2,

    /// <summary>Pressing a rotary encoder. Includes the Loupedeck CT wheel (its press, swipes
    /// and mode menu) unless the provider opts out through
    /// <see cref="IExclusiveModeProvider.WheelFollowsRotaryScope"/>.</summary>
    RotaryPress = 1 << 3,

    /// <summary>The hardware (LED) buttons.</summary>
    SimpleButtons = 1 << 4,

    /// <summary>The Loupedeck CT wheel (rotary index 6) — turning, pressing, swiping and its mode
    /// menu, independent of <see cref="RotaryTurn"/> and <see cref="RotaryPress"/>. The wheel screen
    /// is drawn through <see cref="IExclusiveModeProvider.RenderWheel"/>. No effect on devices
    /// without a wheel. Added in SDK 1.32.0; an older host ignores it.</summary>
    Wheel = 1 << 5,

    /// <summary>Every control — a whole-device takeover. The default. Its value is unchanged
    /// since 1.19.0 and does not contain <see cref="Wheel"/>; it takes the CT wheel through
    /// <see cref="RotaryTurn"/> and <see cref="RotaryPress"/>.</summary>
    All = TouchButtons | SideDisplays | RotaryTurn | RotaryPress | SimpleButtons
}
