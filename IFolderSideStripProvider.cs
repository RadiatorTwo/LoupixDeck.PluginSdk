namespace LoupixDeck.PluginSdk;

/// <summary>
/// Optional capability of an <see cref="IFolderProvider"/>: the folder owns the side display strips
/// while it is open. Without it the host keeps the strips blank inside a folder, or draws the
/// <see cref="RotaryOverride.Label"/> / <see cref="RotaryOverride.GetValue"/> indicators when the
/// folder sets them.
///
/// The returned session is a regular <see cref="ISideStripSession"/>, so everything a side-strip
/// provider can do outside a folder works here as well:
/// <list type="bullet">
/// <item><see cref="ISideStripSession.RenderStrip"/> draws the whole strip. Returning <c>false</c>
/// lets the host compose the strip from segments instead.</item>
/// <item>A session that also implements <see cref="ISegmentStripSession"/> draws single segments;
/// a segment it declines shows that dial's <see cref="RotaryOverride"/> label and value.</item>
/// <item>A session that also implements <see cref="IAnimatedSideStripSession"/> is driven by the
/// host's animation scheduler.</item>
/// <item>Taps and vertical swipes on the strip go to <see cref="ISideStripSession.OnStripTapped"/>
/// and <see cref="ISideStripSession.OnStripSwiped"/>; the host never pages rotary pages while a
/// folder is open.</item>
/// <item><see cref="ISideStripSession.StripChanged"/> repaints the strip.</item>
/// </list>
///
/// Lifetime: the host calls <see cref="CreateSideStripSession"/> once per side when the folder
/// becomes the visible folder (opened, or returned to from a sub-folder) and disposes the session
/// when another folder becomes visible, the folder closes, or something else takes the strips over
/// (device off, screensaver, full-display renderer, exclusive mode). It creates a fresh session when
/// the folder is shown again after such a takeover.
///
/// In the <see cref="SideStripContext"/>, <see cref="SideStripContext.Rotaries"/> describes the
/// folder's dials (label and value from <see cref="IFolderProvider.RotaryOverrides"/>, no command
/// strings), and <see cref="SideStripContext.RequestNextPage"/> /
/// <see cref="SideStripContext.RequestPreviousPage"/> do nothing, because the rotary pages are
/// frozen while a folder is open. The dials themselves are still driven by
/// <see cref="IFolderProvider.RotaryOverrides"/>.
///
/// Additive since SDK 1.31.0. An older host never asks for the session; the folder still works there
/// with blank strips.
/// </summary>
public interface IFolderSideStripProvider
{
    /// <summary>
    /// Creates the session that drives one strip while this folder is visible, or returns
    /// <c>null</c> to leave that side to the host (blank, or the <see cref="RotaryOverride"/>
    /// indicators). <see cref="SideStripContext.Side"/> says which strip is asked for.
    /// </summary>
    ISideStripSession? CreateSideStripSession(SideStripContext context);
}
