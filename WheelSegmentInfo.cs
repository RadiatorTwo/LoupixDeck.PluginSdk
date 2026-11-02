namespace LoupixDeck.PluginSdk;

/// <summary>
/// How a Loupedeck CT wheel mode divides its round wheel screen into separately pressable segments.
/// The segment order, which is the <see cref="WheelSegmentInfo.Index"/> a command sees, is fixed per
/// mode and given on each member.
/// </summary>
public enum WheelSplitMode
{
    /// <summary>The whole wheel screen is one button. No segments.</summary>
    None,

    /// <summary>Two halves split by a horizontal line: 0 = top, 1 = bottom.</summary>
    Horizontal,

    /// <summary>Two halves split by a vertical line: 0 = left, 1 = right.</summary>
    Vertical,

    /// <summary>Four triangles split by both diagonals (an X): 0 = top, 1 = right, 2 = bottom,
    /// 3 = left.</summary>
    Diagonal,

    /// <summary>Four quadrants split by a horizontal and a vertical line (a +): 0 = top-left,
    /// 1 = top-right, 2 = bottom-left, 3 = bottom-right.</summary>
    Cross
}

/// <summary>
/// The segment of a split Loupedeck CT wheel screen a command runs on, handed to the command as
/// <see cref="CommandContext.WheelSegment"/>.
/// </summary>
/// <param name="Mode">How the wheel screen is split. Never <see cref="WheelSplitMode.None"/>.</param>
/// <param name="Index">Zero-based segment index, in the order documented on <see cref="WheelSplitMode"/>.</param>
/// <param name="Count">Number of segments of <paramref name="Mode"/>: 2 or 4.</param>
/// <remarks>Added in SDK 1.35.0.</remarks>
public readonly record struct WheelSegmentInfo(WheelSplitMode Mode, int Index, int Count);
