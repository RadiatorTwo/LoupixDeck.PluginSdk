namespace LoupixDeck.PluginSdk;

/// <summary>
/// A command that declares its states (<see cref="CommandDescriptor.States"/>) and tells the host,
/// per button, which of them to show — so two buttons bound to the same command with different
/// parameters (two sources, two scenes) each show their own state.
/// </summary>
/// <remarks>
/// <para>Nothing is polled. The host asks only when a button may show the wrong state: when the
/// command is put on a button, when the button editor closes, when the plugin is loaded, and when
/// the plugin calls <see cref="IPluginHost.RefreshButtonStates"/> because its data changed (for
/// example on an event from the controlled application).</para>
/// <para>Additive since SDK 1.34.0. An older host never calls <see cref="GetButtonState"/>; drive
/// the buttons with <see cref="IPluginHost.SetActiveButtonState"/> there instead.</para>
/// </remarks>
public interface IButtonStateCommand : IPluginCommand
{
    /// <summary>
    /// The state the button in <paramref name="ctx"/> should show — the
    /// <see cref="ButtonStateDescriptor.Name"/> of one of the declared states — or null to leave it
    /// as it is (no data yet). <see cref="CommandContext.Parameters"/> are that button's parameters,
    /// <see cref="CommandContext.StateName"/> the state it shows now. Called on any thread and for
    /// every bound button in turn, so it must be fast, synchronous and free of side effects: answer
    /// from data an event-driven path keeps up to date.
    /// </summary>
    string? GetButtonState(CommandContext ctx);
}
