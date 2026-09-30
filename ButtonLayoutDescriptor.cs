namespace LoupixDeck.PluginSdk;

/// <summary>Which layers the host creates when a command is put on a touch button.</summary>
public enum ButtonLayoutMode
{
    /// <summary>The host's standard look: the command's icon with its display name as a caption below
    /// (the caption alone, larger, when the command declares no icon).</summary>
    Default = 0,

    /// <summary>No layers at all. For a command that renders the whole button itself, such as an
    /// <see cref="IDisplayImageCommand"/>: the button carries the command and nothing else.</summary>
    None = 1,

    /// <summary>Only the icon, centred on the key.</summary>
    IconOnly = 2,

    /// <summary>Only the display name as a caption, filling the key.</summary>
    CaptionOnly = 3,

    /// <summary>Icon with the caption below it. Same as <see cref="Default"/> for a command that
    /// declares an icon; unlike <see cref="Default"/> it never falls back to a bare caption.</summary>
    IconAndCaption = 4,

    /// <summary>The layers listed in <see cref="ButtonLayoutDescriptor.Layers"/>, in that order
    /// (first is the bottom layer).</summary>
    Custom = 5
}

/// <summary>The kind of a <see cref="ButtonLayerDescriptor"/>.</summary>
public enum ButtonLayerKind
{
    /// <summary>An icon drawn from <see cref="ButtonLayerDescriptor.Glyph"/>.</summary>
    Symbol = 0,

    /// <summary>A text caption drawn from <see cref="ButtonLayerDescriptor.Text"/>.</summary>
    Text = 1
}

/// <summary>
/// One layer a command brings along when it is put on a touch button
/// (<see cref="ButtonLayoutMode.Custom"/>). Sizes and offsets are given for a 90 px key and scaled by the
/// host onto the key actually being written, so the same descriptor fits every device.
/// </summary>
/// <remarks>Additive since SDK 1.27.0.</remarks>
public sealed class ButtonLayerDescriptor
{
    /// <summary>Whether this layer is an icon or a text.</summary>
    public ButtonLayerKind Kind { get; init; } = ButtonLayerKind.Symbol;

    /// <summary>Layer name shown in the editor's layer list. Null uses the command's display name.</summary>
    public string? Name { get; init; }

    /// <summary>Symbol layers: a Material Design Icons glyph (a single code point). Null uses the
    /// command's <see cref="CommandDescriptor.Icon"/>. A glyph the host cannot resolve leaves the
    /// layer out.</summary>
    public string? Glyph { get; init; }

    /// <summary>Text layers: the caption. Null uses the command's display name.</summary>
    public string? Text { get; init; }

    /// <summary>Text layers: font size in pixels on a 90 px key.</summary>
    public int TextSize { get; init; } = 14;

    /// <summary>Symbol layers: the fraction of the key's short edge the icon fills (0.1 – 1.0).</summary>
    public double IconScale { get; init; } = 0.5;

    /// <summary>Horizontal offset from the key centre in pixels on a 90 px key; negative is left.</summary>
    public int OffsetX { get; init; }

    /// <summary>Vertical offset from the key centre in pixels on a 90 px key; negative is up.</summary>
    public int OffsetY { get; init; }

    /// <summary>Text layers: width of the text box on a 90 px key. 0 fills the key.</summary>
    public int BoxWidth { get; init; }

    /// <summary>Text layers: height of the text box on a 90 px key. 0 fills the key.</summary>
    public int BoxHeight { get; init; }

    /// <summary>Colour as <c>#RRGGBB</c> or <c>#AARRGGBB</c>. Null keeps the host's default (white).</summary>
    public string? Color { get; init; }
}

/// <summary>
/// Describes the layers the host creates when a command is put on a touch button, from the button editor
/// or from the actions panel alike. A command that sets no layout keeps the host's default look.
/// </summary>
/// <remarks>
/// Additive since SDK 1.27.0 — a command that declares no layout behaves exactly as before. The layers
/// are created once, at assignment; they are ordinary layers the user may edit afterwards, and an
/// <see cref="IDisplayImageCommand"/> or <see cref="IDisplayCommand"/> still renders on top at runtime.
/// </remarks>
public sealed class ButtonLayoutDescriptor
{
    /// <summary>Which layers to create. Defaults to <see cref="ButtonLayoutMode.Default"/>.</summary>
    public ButtonLayoutMode Mode { get; init; } = ButtonLayoutMode.Default;

    /// <summary>The layers for <see cref="ButtonLayoutMode.Custom"/>; ignored for every other mode.</summary>
    public IReadOnlyList<ButtonLayerDescriptor> Layers { get; init; } = [];
}
