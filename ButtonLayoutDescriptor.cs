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

    /// <summary>Only the icon, centred on the key. A command whose icon the host cannot resolve gets
    /// the standard look instead, so the button is never left empty.</summary>
    IconOnly = 2,

    /// <summary>Only the display name as a caption, filling the key.</summary>
    CaptionOnly = 3,

    /// <summary>Icon with the caption below it. Behaves like <see cref="Default"/>; it lets a plugin
    /// state the choice explicitly, so a later change to the host default cannot alter it.</summary>
    IconAndCaption = 4,

    /// <summary>The layers listed in <see cref="ButtonLayoutDescriptor.Layers"/>, in that order
    /// (first is the bottom layer).</summary>
    Custom = 5
}

/// <summary>The kind of a <see cref="ButtonLayerDescriptor"/>.</summary>
public enum ButtonLayerKind
{
    /// <summary>An icon: a Material Design Icons glyph from <see cref="ButtonLayerDescriptor.Glyph"/>,
    /// or — when <see cref="ButtonLayerDescriptor.ImageData"/> is set — an icon drawn from that SVG or
    /// bitmap. Either way it is a symbol layer the user can tint like any other icon.</summary>
    Symbol = 0,

    /// <summary>A text caption drawn from <see cref="ButtonLayerDescriptor.Text"/>.</summary>
    Text = 1,

    /// <summary>A picture drawn from <see cref="ButtonLayerDescriptor.ImageData"/>, shown in its own
    /// colours like an image the user added. Without image data the layer is left out.</summary>
    Image = 2,

    /// <summary>
    /// An arc showing the value of an <see cref="IValueDisplayCommand"/>: a track over the whole sweep
    /// and a fill (<see cref="ButtonLayerDescriptor.Color"/>) up to the value. Sized by
    /// <see cref="ButtonLayerDescriptor.IconScale"/> and styled by <see cref="ButtonLayerDescriptor.TrackColor"/>,
    /// <see cref="ButtonLayerDescriptor.Thickness"/>, <see cref="ButtonLayerDescriptor.StartAngle"/> and
    /// <see cref="ButtonLayerDescriptor.SweepAngle"/>. It draws nothing while the command reports no
    /// value. Additive since SDK 1.29.0; an older host leaves the layer out.
    /// </summary>
    Indicator = 3
}

/// <summary>Where a text layer of a <see cref="ButtonLayoutDescriptor"/> takes its text from.</summary>
/// <remarks>Additive since SDK 1.29.0.</remarks>
public enum ButtonTextSource
{
    /// <summary>The layer's own <see cref="ButtonLayerDescriptor.Text"/>.</summary>
    Text = 0,

    /// <summary>The <see cref="AdjustmentValue.Text"/> the command reports, e.g. <c>"67%"</c>.</summary>
    Value = 1,

    /// <summary>The <see cref="AdjustmentValue.Detail"/> the command reports, e.g. <c>"resets in 3h12"</c>.</summary>
    Detail = 2
}

/// <summary>
/// One layer a command brings along when it is put on a touch button
/// (<see cref="ButtonLayoutMode.Custom"/>). Sizes and offsets are given for a 90 px key and scaled by the
/// host onto the key actually being written, so the same descriptor fits every device.
/// </summary>
/// <remarks>Additive since SDK 1.27.0.</remarks>
public sealed class ButtonLayerDescriptor
{
    /// <summary>Whether this layer is an icon, a text or a picture.</summary>
    public ButtonLayerKind Kind { get; init; } = ButtonLayerKind.Symbol;

    /// <summary>Layer name shown in the editor's layer list. Null uses the command's display name.</summary>
    public string? Name { get; init; }

    /// <summary>Symbol layers: a Material Design Icons glyph (a single code point). Null uses the
    /// command's <see cref="CommandDescriptor.Icon"/>. A glyph the host cannot resolve leaves the
    /// layer out.</summary>
    public string? Glyph { get; init; }

    /// <summary>
    /// Symbol and image layers: the file content of an SVG or a bitmap (PNG, JPEG, WebP, GIF), for an
    /// icon that is not part of the Material Design Icons catalog. The host recognises the format from
    /// the bytes and stores the picture in its asset store when the command is assigned, so the
    /// button keeps working without the plugin. On a symbol layer it takes precedence over
    /// <see cref="Glyph"/>. Keep it small — an icon, not a photo: the bytes stay in memory with the
    /// descriptor for as long as the plugin is loaded. Data the host cannot decode leaves the layer out.
    /// </summary>
    public byte[]? ImageData { get; init; }

    /// <summary>
    /// Symbol layers drawn from <see cref="ImageData"/>: keep the picture's own colours instead of
    /// tinting it. Null lets the host decide — a single-colour picture is tinted like a glyph, a
    /// multi-colour one keeps its colours. Ignored for glyphs and for image layers, which always
    /// keep their colours.
    /// </summary>
    public bool? KeepOriginalColors { get; init; }

    /// <summary>Text layers: the caption. Null uses the command's display name.</summary>
    public string? Text { get; init; }

    /// <summary>Text layers: font size in pixels on a 90 px key.</summary>
    public int TextSize { get; init; } = 14;

    /// <summary>Symbol and image layers: the fraction of the key's short edge the picture fills
    /// (0.1 – 1.0), keeping its aspect ratio. Indicator layers: the fraction of the short edge the
    /// arc's square box fills.</summary>
    public double IconScale { get; init; } = 0.5;

    /// <summary>Horizontal offset from the key centre in pixels on a 90 px key; negative is left.</summary>
    public int OffsetX { get; init; }

    /// <summary>Vertical offset from the key centre in pixels on a 90 px key; negative is up.</summary>
    public int OffsetY { get; init; }

    /// <summary>Text layers: width of the text box on a 90 px key. 0 fills the key.</summary>
    public int BoxWidth { get; init; }

    /// <summary>Text layers: height of the text box on a 90 px key. 0 fills the key.</summary>
    public int BoxHeight { get; init; }

    /// <summary>Colour as <c>#RRGGBB</c> or <c>#AARRGGBB</c>. Null keeps the host's default (white).
    /// For an indicator layer this is the fill.</summary>
    public string? Color { get; init; }

    /// <summary>
    /// Text layers: where the text comes from. <see cref="ButtonTextSource.Value"/> and
    /// <see cref="ButtonTextSource.Detail"/> show what an <see cref="IValueDisplayCommand"/> reports, so
    /// the text keeps every styling option the user has for text. Additive since SDK 1.29.0; an older
    /// host shows <see cref="Text"/> instead.
    /// </summary>
    public ButtonTextSource TextSource { get; init; } = ButtonTextSource.Text;

    /// <summary>Indicator layers: colour of the unfilled track as <c>#RRGGBB</c> or <c>#AARRGGBB</c>.
    /// Null keeps the host's default. Additive since SDK 1.29.0.</summary>
    public string? TrackColor { get; init; }

    /// <summary>Indicator layers: stroke width as a fraction of the arc's box (e.g. 0.08). Null keeps
    /// the host's default. Additive since SDK 1.29.0.</summary>
    public double? Thickness { get; init; }

    /// <summary>Indicator layers: where the arc starts, in degrees clockwise from 3 o'clock (-90 is
    /// 12 o'clock). Null keeps the host's default, 135, which leaves the gap at the bottom.
    /// Additive since SDK 1.29.0.</summary>
    public double? StartAngle { get; init; }

    /// <summary>Indicator layers: how far the arc sweeps clockwise, in degrees (360 is a full ring).
    /// Null keeps the host's default, 270. Additive since SDK 1.29.0.</summary>
    public double? SweepAngle { get; init; }
}

/// <summary>
/// Describes the layers the host creates when a command is put on a touch button — from the actions panel
/// and, for a command that declares one, from the button editor. A command that sets no layout is unchanged.
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

    /// <summary>
    /// Background colour the button gets with the layers, as <c>#RRGGBB</c>. It is the button's own
    /// background setting, so the user changes it like any other. Null leaves the background as it
    /// is. Applies to every mode. Additive since SDK 1.29.0.
    /// </summary>
    public string? BackgroundColor { get; init; }
}
