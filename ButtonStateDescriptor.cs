namespace LoupixDeck.PluginSdk;

/// <summary>
/// One state a command brings along. A command that declares states owns the state set of every
/// button it is assigned to: the host materializes exactly these states (in this order) when the
/// command is inserted, locks state management in the editor, and drives the active state through
/// <see cref="IPluginHost.SetActiveButtonState"/>. The user keeps full control over the layers
/// inside each state.
/// </summary>
/// <remarks>
/// Additive since SDK 1.21.0 — a command that declares no states behaves exactly as before.
/// </remarks>
public sealed class ButtonStateDescriptor
{
    /// <summary>
    /// State name, unique within the command. Persisted in the button config and the identifier
    /// <see cref="IPluginHost.SetActiveButtonState"/> and <see cref="CommandContext.StateName"/>
    /// use — it MUST never change once the plugin has shipped.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>Optional one-line explanation shown next to the state in the editor.</summary>
    public string? Description { get; init; }
}
