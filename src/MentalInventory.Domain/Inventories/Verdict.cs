namespace MentalInventory.Domain.Inventories;

/// <summary>
/// The outcome of reviewing an item. Each item is tested with two questions:
/// "Is this vital?" and "Does this matter?" If neither, it is struck.
/// </summary>
/// <remarks>The numeric values are part of the persisted events. Never renumber them.</remarks>
public enum Verdict
{
    /// <summary>Kept because it is vital (rent, taxes, health, work).</summary>
    Vital = 1,

    /// <summary>Kept because it matters, to me or to someone I care about.</summary>
    Matters = 2,

    /// <summary>Neither vital nor mattering. Struck from the list.</summary>
    Struck = 3,
}
