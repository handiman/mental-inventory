namespace MentalInventory.Domain.Inventories;

/// <summary>The three columns of a mental inventory.</summary>
/// <remarks>The numeric values are part of the persisted events. Never renumber them.</remarks>
public enum Column
{
    /// <summary>Things I'm working on.</summary>
    WorkingOn = 1,

    /// <summary>Things I should be working on.</summary>
    ShouldBeWorkingOn = 2,

    /// <summary>Things I want to be working on.</summary>
    WantToBeWorkingOn = 3,
}
