namespace LoupixDeck.PluginSdk;

/// <summary>
/// Result of a single <see cref="IFullDisplayRenderer.RenderFrame"/> call. Tells the host whether
/// the renderer produced a new frame this tick (so the host can skip an unnecessary full-display
/// blit) and whether the stream has finished (a one-shot source freezes on its last frame).
///
/// Use the factory helpers instead of constructing directly:
/// <list type="bullet">
///   <item><see cref="Skip"/> — no new frame was ready this tick; the host keeps the last one.</item>
///   <item><see cref="Frame"/> — a frame was written to the buffer; the stream continues.</item>
///   <item><see cref="Final"/> — the last frame was written; the host stops ticking and holds it.</item>
/// </list>
/// </summary>
public readonly struct FullDisplayFrameResult
{
    /// <summary>
    /// Monotonic index of the content frame the renderer wrote. The host treats this as the dirty
    /// key: it only pushes to the device when this value differs from the previously pushed one, so
    /// returning the same number twice costs no device I/O.
    /// </summary>
    public long FrameNumber { get; }

    /// <summary>True when the renderer wrote a frame to the buffer this tick; false to leave the
    /// display unchanged.</summary>
    public bool Drawn { get; }

    /// <summary>True when this is the final frame of a one-shot source. The host stops ticking the
    /// renderer and keeps the last frame on screen (the session stays open until released).</summary>
    public bool IsFinal { get; }

    private FullDisplayFrameResult(long frameNumber, bool drawn, bool isFinal)
    {
        FrameNumber = frameNumber;
        Drawn = drawn;
        IsFinal = isFinal;
    }

    /// <summary>No new frame this tick — the host leaves the display unchanged.</summary>
    public static FullDisplayFrameResult Skip() => new(0, drawn: false, isFinal: false);

    /// <summary>A frame numbered <paramref name="frameNumber"/> was written; the stream continues.</summary>
    public static FullDisplayFrameResult Frame(long frameNumber) => new(frameNumber, drawn: true, isFinal: false);

    /// <summary>The final frame (<paramref name="frameNumber"/>) was written; the host stops ticking
    /// and holds it.</summary>
    public static FullDisplayFrameResult Final(long frameNumber) => new(frameNumber, drawn: true, isFinal: true);
}
