namespace LoupixDeck.PluginSdk;

/// <summary>
/// A plugin-provided renderer that fills the ENTIRE device display with raw BGRA frames, driven by
/// the host's central animation scheduler at <see cref="TargetFps"/>. Unlike
/// <see cref="IExclusiveModeProvider"/> — which delivers per-slot PNG/<see cref="FolderEntry"/>
/// tiles the host must decode and composite — this delivers one contiguous framebuffer per frame,
/// the high-throughput path for video and continuously animated full-screen content.
///
/// Acquire the display via <see cref="IPluginHost.RequestFullDisplayRenderer"/>; the returned
/// <see cref="IFullDisplayRenderSession"/> owns the lifetime. The renderer keeps decoding/producing
/// off the scheduler thread and simply copies the freshest frame into the host buffer each tick.
///
/// Backward compatibility: this is an independent, optional interface. Existing plugins neither
/// implement nor reference it and are unaffected.
/// </summary>
public interface IFullDisplayRenderer
{
    /// <summary>
    /// Desired frame rate. The host clamps this to its global animation FPS limit, so the effective
    /// rate may be lower; read <see cref="FullDisplayFrameContext.EffectiveFps"/> for the actual
    /// rate. A value &lt;= 0 means "use the host's default limit".
    /// </summary>
    int TargetFps { get; }

    /// <summary>
    /// Whether the renderer currently wants frames. Flipping to false pauses the scheduler tick
    /// cheaply while keeping decoding warm; the host does NOT release the session. The host also
    /// stops ticking (without releasing) when the owning device goes inactive.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Called once, off the UI thread, when the session is granted, with the exact target surface
    /// geometry. Start decoding/producing here so the first <see cref="RenderFrame"/> has content.
    /// </summary>
    void OnStart(FullDisplaySurface surface);

    /// <summary>
    /// Called once when the session is released — plugin <see cref="IFullDisplayRenderSession.Release"/>,
    /// plugin unload/<see cref="LoupixPlugin.Shutdown"/>, or host teardown. Stop decoding and free
    /// resources here.
    /// </summary>
    void OnStop();

    /// <summary>
    /// Fills <paramref name="buffer"/> with one frame for the current surface. The buffer is
    /// host-owned/pooled and holds at least <c>Stride * Height</c> bytes — write exactly that many
    /// and never read <c>buffer.Length</c> (a pooled buffer may be larger). Called off the UI thread
    /// at up to <see cref="TargetFps"/>; must be fast and non-blocking (do decode work asynchronously
    /// and copy the freshest frame here). Return <see cref="FullDisplayFrameResult.Skip"/> when no
    /// new frame is ready (the host skips the device push), or
    /// <see cref="FullDisplayFrameResult.Frame"/> / <see cref="FullDisplayFrameResult.Final"/> with a
    /// monotonic frame number the host uses to dirty-check the push.
    /// </summary>
    FullDisplayFrameResult RenderFrame(byte[] buffer, in FullDisplayFrameContext frame);
}
