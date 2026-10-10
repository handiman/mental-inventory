namespace MentalInventory.Domain.Inventories;

/// <summary>
/// A fact that happened to an inventory. Events are persisted and replayed, possibly years
/// later and in a user's browser we can't migrate, so treat them as a public schema:
/// never change or remove a field, add a new event type (or version) instead.
/// </summary>
public interface IInventoryEvent
{
}

public sealed record InventoryStarted(InventoryId InventoryId, DateTimeOffset StartedAt) : IInventoryEvent;

public sealed record ItemAdded(ItemId ItemId, Column Column, string Text) : IInventoryEvent;

public sealed record ItemReworded(ItemId ItemId, string Text) : IInventoryEvent;

public sealed record ItemMoved(ItemId ItemId, Column To) : IInventoryEvent;

public sealed record ItemRemoved(ItemId ItemId) : IInventoryEvent;

public sealed record ReviewStarted : IInventoryEvent;

public sealed record ItemReviewed(ItemId ItemId, Verdict Verdict) : IInventoryEvent;

public sealed record InventoryCompleted(DateTimeOffset CompletedAt) : IInventoryEvent;
