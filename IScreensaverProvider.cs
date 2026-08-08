namespace LoupixDeck.PluginSdk;

/// <summary>
/// A plugin-supplied, discoverable source for the host's idle screensaver. The user picks one in
/// Settings → Screensaver; the host starts it when the idle timeout elapses and stops it on the
/// first device input.
///
/// The provider itself is a stateless descriptor + factory: the host calls
/// <see cref="CreateRenderer"/> once per screensaver run and drops the renderer after
/// <see cref="IFullDisplayRenderer.OnStop"/>, so the same provider can serve repeated runs and more
/// than one device.
///
/// The frames come from an ordinary <see cref="IFullDisplayRenderer"/> — same host-owned buffer, same
/// <see cref="FullDisplayFrameResult"/> dirty key, same central animation scheduler. The difference
/// is ownership: the HOST drives the lifetime here, so there is no
/// <see cref="IFullDisplayRenderSession"/> and no <see cref="IPluginHost.RequestFullDisplayRenderer"/>
/// call. Unlike the host's built-in video screensaver, this path does not need ffmpeg.
///
/// Backward compatibility: this is an independent, optional interface. Existing plugins neither
/// implement nor reference it and are unaffected.
/// </summary>
public interface IScreensaverProvider
{
    /// <summary>Stable, unique id persisted in the host config. Recommended form:
    /// <c>"{pluginId}.{name}"</c> to avoid collisions across plugins. A config referring to an id
    /// that no longer resolves simply means "no screensaver" — the host logs and keeps the normal
    /// page on screen.</summary>
    string Id { get; }

    /// <summary>Human-readable label shown in the Settings screensaver picker.</summary>
    string Title { get; }

    /// <summary>
    /// Creates a fresh renderer for one screensaver run. Called off the UI thread when the idle
    /// timeout fires. Return <c>null</c> to decline (the host then skips the screensaver for this
    /// run). The host calls <see cref="IFullDisplayRenderer.OnStart"/>,
    /// <see cref="IFullDisplayRenderer.RenderFrame"/> and <see cref="IFullDisplayRenderer.OnStop"/>
    /// on the returned instance and never reuses it afterwards.
    ///
    /// Returning <see cref="FullDisplayFrameResult.Final"/> from a frame ends the screensaver: the
    /// host pushes that last frame, stops the renderer and repaints the active page — the same
    /// behavior a non-looping video clip has.
    /// </summary>
    IFullDisplayRenderer CreateRenderer();
}
