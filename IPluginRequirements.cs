namespace LoupixDeck.PluginSdk;

/// <summary>
/// One system requirement of a plugin, for example an external tool that must be installed.
/// Returned from <see cref="IPluginRequirements.GetRequirements"/>. All texts are authored in
/// English; the host translates them through the plugin's own <c>strings.&lt;code&gt;.json</c>
/// files, keyed by the English text, exactly like command descriptors.
/// </summary>
public sealed class PluginRequirement
{
    /// <summary>Stable identifier of the requirement (for example <c>pactl</c>). The host uses it
    /// to remember which unmet requirements the user has already been told about, so it must not
    /// change between releases.</summary>
    public required string Id { get; init; }

    /// <summary>Short display name (for example <c>pactl (PulseAudio utilities)</c>).</summary>
    public required string Name { get; init; }

    /// <summary>Whether the requirement is currently satisfied.</summary>
    public required bool IsMet { get; init; }

    /// <summary>What is wrong when <see cref="IsMet"/> is <see langword="false"/>. Ignored when
    /// the requirement is met.</summary>
    public string? Message { get; init; }

    /// <summary>Optional hint on how to fix it (for example an install command).</summary>
    public string? InstallHint { get; init; }
}

/// <summary>
/// Optional interface a <see cref="LoupixPlugin"/> may implement to tell the host which system
/// requirements it has, so the user learns why a plugin does not work instead of seeing a plugin
/// that is loaded but silent.
///
/// The host calls <see cref="GetRequirements"/> after the plugin has loaded and again on demand
/// (for example when the Plugins page or the Linux Doctor refreshes), always off the startup
/// path. Implementations should be quick and must not throw; a failing call is treated as "no
/// requirements reported".
///
/// Backward compatibility: this is an independent, optional interface. A plugin that does not
/// implement it behaves exactly as before.
/// </summary>
public interface IPluginRequirements
{
    /// <summary>Evaluates the requirements right now. Include met requirements as well so the
    /// host can show the complete picture; only entries with <see cref="PluginRequirement.IsMet"/>
    /// set to <see langword="false"/> raise a notice.</summary>
    IReadOnlyList<PluginRequirement> GetRequirements();
}
