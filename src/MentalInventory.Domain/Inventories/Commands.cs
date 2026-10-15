namespace MentalInventory.Domain.Inventories;

/// <summary>An intent to change an inventory. Decided by <see cref="InventoryDecider.Decide"/>.</summary>
public interface IInventoryCommand
{
}

public sealed record StartInventory(InventoryId InventoryId, DateTimeOffset Now) : IInventoryCommand;

public sealed record AddItem(ItemId ItemId, Column Column, ItemText Text) : IInventoryCommand;

public sealed record RewordItem(ItemId ItemId, ItemText Text) : IInventoryCommand;

public sealed record MoveItem(ItemId ItemId, Column To) : IInventoryCommand;

/// <summary>Removes an item added by mistake. Not the same as striking it in review.</summary>
public sealed record RemoveItem(ItemId ItemId) : IInventoryCommand;

public sealed record StartReview : IInventoryCommand;

public sealed record ReviewItem(ItemId ItemId, Verdict Verdict) : IInventoryCommand;

public sealed record CompleteInventory(DateTimeOffset Now) : IInventoryCommand;
