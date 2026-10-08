namespace LoupixDeck.PluginSdk;

/// <summary>
/// A plugin command that reports a value for the touch button it is on to draw with the button's
/// own layers — a gauge, a level, a progress. The host polls <see cref="GetValue"/> and hands the
/// result to the button's indicator layers (the arc follows <see cref="AdjustmentValue.Normalized"/>)
/// and to text layers set to show the value (<see cref="AdjustmentValue.Text"/>) or its detail
/// (<see cref="AdjustmentValue.Detail"/>). Every layer stays an ordinary layer the user can move and
/// restyle; the plugin supplies only the numbers. Declare the starting layers with
/// <see cref="CommandDescriptor.ButtonLayout"/> (<see cref="ButtonLayerKind.Indicator"/> and
/// <see cref="ButtonLayerDescriptor.TextSource"/>). Set <see cref="IPluginCommand.SupportedTargets"/>
/// to <see cref="ButtonTargets.TouchButton"/>.
/// </summary>
/// <remarks>
/// Additive since SDK 1.29.0. It combines with <see cref="IDisplayCommand"/> or
/// <see cref="IDisplayImageCommand"/>: the value feeds the button's layers and the display command
/// still draws its own layer, both on <see cref="UpdateInterval"/>.
/// <see cref="IPluginHost.RequestButtonRefresh"/> re-reads the value immediately.
/// </remarks>
public interface IValueDisplayCommand : IPluginCommand
{
    /// <summary>How often the host polls <see cref="GetValue"/>.</summary>
    TimeSpan UpdateInterval { get; }

    /// <summary>
    /// The current value, or null when there is none (indicators and value text then draw nothing).
    /// Use <see cref="double.NaN"/> as <see cref="AdjustmentValue.Normalized"/> for text without an
    /// arc. Called on the polling timer, so it must be fast, synchronous and free of side effects:
    /// return a cached value that an asynchronous data path keeps up to date.
    /// </summary>
    AdjustmentValue? GetValue(CommandContext ctx);
}
