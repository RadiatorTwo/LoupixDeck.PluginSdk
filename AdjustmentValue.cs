namespace LoupixDeck.PluginSdk;

/// <summary>
/// The current value of an <see cref="IAdjustmentCommand"/>, as the host needs it to draw a
/// dial indicator: a position on a scale plus the text that names it. Deliberately free of any
/// domain meaning — a volume, a brightness, a zoom factor and a fan speed all reduce to the
/// same two fields, and a renderer that knows which one it is drawing could not stay generic.
/// </summary>
/// <param name="Normalized">
/// Position on the command's own scale, 0 (minimum) to 1 (maximum). The host clamps it, so a
/// value outside the range is a harmless rounding artefact rather than a rendering bug.
/// </param>
/// <param name="Text">
/// Short display text for the value, e.g. <c>"75%"</c>, <c>"-12 dB"</c> or a mute glyph.
/// <c>null</c> draws the bar without a caption.
/// </param>
public readonly record struct AdjustmentValue(double Normalized, string? Text);
