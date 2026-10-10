using System.Collections.Immutable;

namespace MentalInventory.Domain.Inventories;

/// <summary>Where an inventory is in its lifecycle.</summary>
public enum Phase
{
    /// <summary>No events yet.</summary>
    NotStarted,

    /// <summary>Writing things down in the three columns.</summary>
    Listing,

    /// <summary>Asking "is it vital, does it matter?" of each item.</summary>
    Reviewing,

    /// <summary>Done. The inventory is read-only.</summary>
    Completed,
}

/// <summary>An item in an inventory. <see cref="Verdict"/> is null until reviewed.</summary>
public sealed record Item(ItemId Id, Column Column, string Text, Verdict? Verdict = null)
{
    public bool IsKept => Verdict is Inventories.Verdict.Vital or Inventories.Verdict.Matters;
}

/// <summary>
/// The current state of one inventory, folded from its events. Immutable, so the UI can
/// render it directly and keep the previous state around for undo.
/// </summary>
public sealed record Inventory(
    InventoryId Id,
    Phase Phase,
    ImmutableList<Item> Items,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt)
{
    public static Inventory Initial { get; } = new(default, Phase.NotStarted, [], null, null);

    public Item? Find(ItemId id) => Items.Find(item => item.Id == id);

    public IEnumerable<Item> InColumn(Column column) => Items.Where(item => item.Column == column);

    /// <summary>The items that survived review. This is what goes into the journal.</summary>
    public IEnumerable<Item> Kept => Items.Where(item => item.IsKept);

    public bool IsFullyReviewed => Items.All(item => item.Verdict is not null);
}
