namespace MentalInventory.Domain.Inventories;

/// <summary>A command was rejected because it breaks a rule of the inventory.</summary>
public sealed class InventoryRuleViolation(string message) : InvalidOperationException(message);
