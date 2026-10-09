namespace LoupixDeck.PluginSdk;

/// <summary>
/// One slot inside a folder view. Tapping the slot either runs
/// <see cref="OnPress"/> or opens the nested <see cref="OpensFolder"/>.
/// </summary>
public sealed class FolderEntry
{
    /// <summary>Target grid slot (see <see cref="FolderLayout"/>).</summary>
    public int SlotIndex { get; init; }

    public string Text { get; init; } = string.Empty;

    /// <summary>Optional PNG-encoded icon bytes; null when the slot is text-only.</summary>
    public byte[]? Image { get; init; }

    /// <summary>
    /// Optional callback that draws the slot itself, on a canvas of the key's real size
    /// (<see cref="IRenderCanvas.Width"/> x <see cref="IRenderCanvas.Height"/>, which differs between
    /// devices and key calibrations). Use it instead of <see cref="Image"/> when the slot has to be
    /// pixel-exact: a PNG is a fixed size the host has to scale to the key, the canvas is not, and
    /// <see cref="IRenderCanvas.DrawPixels"/> puts a plugin-owned framebuffer on it unscaled.
    /// The slot is composed in this order: background colour, <see cref="Image"/>, this callback,
    /// then <see cref="Text"/>.
    /// The host calls it every time it repaints the folder — after each
    /// <see cref="IFolderProvider.EntriesChanged"/> and whenever the device redraws — while holding
    /// its render lock, so draw from state the entry already captured and return quickly; never block,
    /// never touch the UI. The canvas is only valid during the call. An exception is caught and logged
    /// by the host, and the slot is shown without this layer.
    /// Additive since SDK 1.28.0. A host built against an older SDK ignores the member, so a plugin that
    /// must also run there keeps setting <see cref="Image"/> and checks <see cref="SdkInfo.Version"/>.
    /// </summary>
    public Action<IRenderCanvas>? Render { get; init; }

    public PluginColor BackColor { get; init; } = PluginColor.Black;
    public PluginColor TextColor { get; init; } = PluginColor.White;
    public int TextSize { get; init; } = 16;
    public bool Bold { get; init; }

    /// <summary>Action run when the slot is tapped (a leaf entry).</summary>
    public Func<Task>? OnPress { get; init; }

    /// <summary>Nested folder opened when the slot is tapped (a folder entry).</summary>
    public IFolderProvider? OpensFolder { get; init; }
}

/// <summary>Per-rotary-encoder behavior override while a folder is open.</summary>
public sealed class RotaryOverride
{
    public Func<Task>? OnLeft { get; init; }
    public Func<Task>? OnRight { get; init; }
    public Func<Task>? OnPress { get; init; }

    /// <summary>
    /// Optional label the host draws in this dial's side-strip segment while the folder is open,
    /// the same way it draws a configured dial's label outside a folder. Only side-strip devices
    /// show it. Additive since SDK 1.31.0; an older host ignores it.
    /// </summary>
    public string? Label { get; init; }

    /// <summary>
    /// Optional live value the host draws in this dial's side-strip segment as the standard dial
    /// indicator (a bar with the value text, under <see cref="Label"/>), exactly like a dial bound to
    /// an <see cref="IAdjustmentCommand"/>. The host pulls it on every strip repaint, outside its
    /// render lock. The host repaints the strip after <see cref="OnLeft"/> / <see cref="OnRight"/> ran;
    /// when the value moves for another reason, raise <see cref="IFolderProvider.EntriesChanged"/>.
    /// Return <c>null</c> to show only the label. Additive since SDK 1.31.0.
    /// </summary>
    public Func<AdjustmentValue?>? GetValue { get; init; }
}
