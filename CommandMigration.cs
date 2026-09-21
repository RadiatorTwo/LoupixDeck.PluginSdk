namespace LoupixDeck.PluginSdk;

/// <summary>
/// Rewrites an old rotary binding to a new command, once, in the user's configuration.
/// <para>
/// A plugin that replaces a set of per-gesture commands with a single
/// <see cref="IAdjustmentCommand"/> would otherwise leave every dial its users already
/// configured on the old commands: they keep working, but none of them gain the new
/// behaviour, and the plugin can never retire the old shape. A migration lets the plugin
/// describe that replacement declaratively and have the host apply it.
/// </para>
/// <para>
/// The host only rewrites a dial whose gestures carry <em>exactly</em> the commands named in
/// <see cref="From"/>. A dial the user has since edited does not match and stays untouched,
/// and the configuration file is backed up before the first rewrite.
/// </para>
/// </summary>
public sealed class CommandMigration
{
    /// <summary>
    /// Stable id, unique within the plugin. The host records <c>"{pluginId}:{Id}"</c> in the
    /// device configuration once the rule has run and never runs it again — so a user who
    /// deliberately rebuilds the old binding by hand keeps it. Never reuse an id for a
    /// different rule; a new replacement needs a new id.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// The old binding, as one command name per gesture — no parameters, just the name. Every
    /// gesture listed must carry exactly that command, as a single command rather than as part
    /// of a chain, and their shared parameter values must agree. A dial that matches only
    /// partly, or whose gestures point at different targets, is left alone.
    /// </summary>
    public required IReadOnlyDictionary<RotaryAction, string> From { get; init; }

    /// <summary>The command name the whole dial is rewritten to, on all three gestures.</summary>
    public required string To { get; init; }

    /// <summary>
    /// Parameter values for the new command, keyed by its parameter name. A value written as
    /// <c>"{oldName}"</c> carries over the value the old binding held for that parameter —
    /// looked up across the matched gestures in turn (counter-clockwise, clockwise, press), so
    /// a value that only one of the old commands declared is still found. Any other value is
    /// taken literally. A parameter that is not listed, or whose source is absent, falls back
    /// to the default the new command declares.
    /// </summary>
    public IReadOnlyDictionary<string, string> Parameters { get; init; }
        = new Dictionary<string, string>();
}
