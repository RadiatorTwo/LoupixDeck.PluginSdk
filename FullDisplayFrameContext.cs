namespace LoupixDeck.PluginSdk;

/// <summary>
/// Timing snapshot handed to <see cref="IFullDisplayRenderer.RenderFrame"/> for one frame. Mirrors
/// the host's internal animation render context (minus host-only types) so a renderer can drive its
/// output from wall-clock time rather than counting ticks. The target <see cref="Surface"/> is
/// carried here too so a renderer does not have to cache the value handed to
/// <see cref="IFullDisplayRenderer.OnStart"/>.
/// </summary>
public readonly struct FullDisplayFrameContext
{
    /// <summary>Zero-based frame counter since the host started ticking this renderer.</summary>
    public long FrameNumber { get; init; }

    /// <summary>Total time elapsed since the host started ticking this renderer.</summary>
    public TimeSpan Elapsed { get; init; }

    /// <summary>Wall-clock time since the previous frame.</summary>
    public TimeSpan Delta { get; init; }

    /// <summary>The rate the host is actually ticking at (after clamping to its global limit).</summary>
    public int EffectiveFps { get; init; }

    /// <summary>Geometry and pixel layout of the buffer to fill this frame.</summary>
    public FullDisplaySurface Surface { get; init; }

    /// <summary>Cancelled when the host is tearing the render loop down; abandon the frame promptly.</summary>
    public CancellationToken CancellationToken { get; init; }
}
