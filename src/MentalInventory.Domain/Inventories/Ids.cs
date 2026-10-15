namespace MentalInventory.Domain.Inventories;

/// <summary>Identifies one mental inventory, which is also its event stream.</summary>
public readonly record struct InventoryId(Guid Value)
{
    /// <summary>A new, time-ordered id. Ids are created by the client, so it works offline.</summary>
    public static InventoryId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}

/// <summary>Identifies an item within an inventory.</summary>
public readonly record struct ItemId(Guid Value)
{
    public static ItemId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
