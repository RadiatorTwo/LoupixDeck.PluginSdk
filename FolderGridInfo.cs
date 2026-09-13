namespace LoupixDeck.PluginSdk;

/// <summary>
/// Key grid of the device the host is currently driving, for folder providers that lay their
/// entries out themselves. The values in <see cref="FolderLayout"/> are a 5x3 default and are
/// wrong on a 4x3 model, so a provider that fills more than a handful of slots must take its
/// geometry from here.
/// </summary>
public sealed record FolderGridInfo(int Columns, int Rows, int BackSlotIndex)
{
    /// <summary>Addressable slots — entries outside this range are dropped by the host.</summary>
    public int TotalSlots => Columns * Rows;

    /// <summary>
    /// Fills the grid in reading order, skipping the reserved back slot, and stops when the
    /// grid is full. Returns the slot index for the n-th entry, or -1 when it does not fit.
    /// </summary>
    public int SlotForIndex(int entryIndex)
    {
        int slot = 0;
        for (int i = 0; i <= entryIndex; i++)
        {
            if (slot == BackSlotIndex) slot++;
            if (slot >= TotalSlots) return -1;
            if (i == entryIndex) return slot;
            slot++;
        }
        return -1;
    }
}
