namespace LoupixDeck.PluginSdk;

/// <summary>
/// Plugin-supplied controller for the global exclusive mode. While a provider
/// is active, the host suppresses normal page-mappings, freezes the folder
/// navigation, and routes the hardware input of the active device to this
/// provider. Use it for device takeovers like a telemetry HUD or a
/// game-specific overlay; for sub-navigation prefer <see cref="IFolderProvider"/>.
/// </summary>
/// <remarks>
/// The takeover does not have to cover the whole device: <see cref="Scope"/> declares
/// which control categories are overridden. Controls outside the scope keep their
/// normal page assignments, and this provider's input callbacks never fire for them.
/// </remarks>
public interface IExclusiveModeProvider
{
    /// <summary>Title shown by the host (e.g. on a status overlay).</summary>
    string Title { get; }

    /// <summary>Called once when the host has accepted the exclusive request.</summary>
    void OnEnter();

    /// <summary>Called once when the provider is released (manually or by shutdown).</summary>
    void OnExit();

    /// <summary>Current touch-slot content (up to the active device's slot count).
    /// Slots not returned are cleared to black by the host. Entries for slots outside
    /// <see cref="Scope"/> are ignored — those slots keep their normal page content.</summary>
    IReadOnlyList<FolderEntry> BuildTouchEntries();

    /// <summary>A simple (hardware) button was pressed. Index is zero-based.
    /// Only raised while <see cref="Scope"/> contains
    /// <see cref="ExclusiveControlScope.SimpleButtons"/>.</summary>
    void OnSimpleButtonPressed(int index);

    /// <summary>A touch slot was tapped. Index is the slot index. Only raised while
    /// <see cref="Scope"/> contains <see cref="ExclusiveControlScope.TouchButtons"/>
    /// (grid slots) resp. <see cref="ExclusiveControlScope.SideDisplays"/> (strip slots).</summary>
    void OnTouchPressed(int slotIndex);

    /// <summary>A rotary encoder was pressed. Only raised while <see cref="Scope"/>
    /// contains <see cref="ExclusiveControlScope.RotaryPress"/>.</summary>
    void OnRotaryPressed(int index);

    /// <summary>A rotary encoder turned. <paramref name="delta"/> is positive for
    /// clockwise / right and negative for counter-clockwise / left. Only raised while
    /// <see cref="Scope"/> contains <see cref="ExclusiveControlScope.RotaryTurn"/>.</summary>
    void OnRotated(int index, int delta);

    /// <summary>Raised when the touch entries (or their displayed data) changed
    /// and the host must redraw the slots. Also the "re-evaluate now" signal after
    /// <see cref="Scope"/> changed.</summary>
    event EventHandler EntriesChanged;

    /// <summary>
    /// Which device controls this provider overrides. Controls outside the scope keep
    /// their normal page assignments — their commands still run, plugin side strips keep
    /// rendering, page swipes keep working. Defaults to <see cref="ExclusiveControlScope.All"/>,
    /// the whole-device takeover that exclusive mode always performed.
    /// </summary>
    /// <remarks>
    /// The host re-reads this property on every input and every redraw and never caches it,
    /// so a provider may change what it overrides while running (e.g. per mode or state).
    /// Raise <see cref="EntriesChanged"/> afterwards to make the host repaint immediately;
    /// input routing follows the new value without it.
    /// </remarks>
    ExclusiveControlScope Scope => ExclusiveControlScope.All;

    /// <summary>
    /// How the host should push this provider's frames to the device. Defaults to
    /// <see cref="ExclusiveRenderMode.FullScreen"/> (one composited blit + DRAW).
    /// Override to opt into per-tile strategies — e.g. <see cref="ExclusiveRenderMode.DirtyTiles"/>
    /// for a telemetry HUD that only changes a few slots per frame.
    /// </summary>
    ExclusiveRenderMode RenderMode => ExclusiveRenderMode.FullScreen;

    /// <summary>
    /// Target slot index for <see cref="ExclusiveRenderMode.SingleTile"/>. Ignored
    /// by every other mode. Defaults to slot 0.
    /// </summary>
    int SingleTileSlot => 0;
}
