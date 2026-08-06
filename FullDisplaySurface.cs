namespace LoupixDeck.PluginSdk;

/// <summary>
/// Pixel layout of a full-display framebuffer the host currently supports. Only
/// <see cref="Bgra8888"/> exists today; the enum leaves room for future formats without
/// changing the <see cref="FullDisplaySurface"/> shape.
/// </summary>
public enum FullDisplayPixelFormat
{
    /// <summary>32-bit BGRA, 8 bits per channel, blue byte first (matches the host device buffers).</summary>
    Bgra8888 = 0
}

/// <summary>
/// Geometry and pixel layout of the full-display framebuffer the host pushes on behalf of an
/// <see cref="IFullDisplayRenderer"/>. A renderer scales its source to exactly
/// <see cref="Width"/>×<see cref="Height"/> and fills the host-provided buffer in
/// <see cref="PixelFormat"/> order, using <see cref="Stride"/> bytes per row.
/// </summary>
public readonly struct FullDisplaySurface
{
    /// <summary>Frame width in device pixels.</summary>
    public int Width { get; init; }

    /// <summary>Frame height in device pixels.</summary>
    public int Height { get; init; }

    /// <summary>Bytes per pixel row. For <see cref="FullDisplayPixelFormat.Bgra8888"/> this is
    /// <c>Width * 4</c>.</summary>
    public int Stride { get; init; }

    /// <summary>Byte layout of each pixel. Currently always
    /// <see cref="FullDisplayPixelFormat.Bgra8888"/>.</summary>
    public FullDisplayPixelFormat PixelFormat { get; init; }
}
