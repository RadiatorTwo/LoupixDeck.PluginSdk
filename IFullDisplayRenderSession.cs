namespace LoupixDeck.PluginSdk;

/// <summary>
/// Handle representing single-owner ownership of the full display, returned by
/// <see cref="IPluginHost.RequestFullDisplayRenderer"/>. Disposing or calling <see cref="Release"/>
/// stops the render loop, calls <see cref="IFullDisplayRenderer.OnStop"/>, and restores the normal
/// page. Releasing deterministically from <see cref="LoupixPlugin.Shutdown"/> is the
/// teardown-on-unload path; the handle also composes with <c>using</c>.
/// </summary>
public interface IFullDisplayRenderSession : IDisposable
{
    /// <summary>True while this session still owns the display; false once released or revoked by
    /// the host (device teardown or losing arbitration).</summary>
    bool IsActive { get; }

    /// <summary>Releases the session. Idempotent; equivalent to <see cref="IDisposable.Dispose"/>.</summary>
    void Release();
}
