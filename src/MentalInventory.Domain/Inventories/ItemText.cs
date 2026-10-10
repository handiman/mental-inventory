namespace MentalInventory.Domain.Inventories;

/// <summary>The validated text of an item: trimmed, not empty, not too long.</summary>
/// <remarks>
/// Used in commands, where input enters the domain. Events store the plain string, so that
/// replaying old events never fails if these rules are tightened later.
/// </remarks>
public sealed record ItemText
{
    public const int MaxLength = 280;

    private ItemText(string value) => Value = value;

    public string Value { get; }

    public static ItemText From(string? text)
    {
        var trimmed = text?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            throw new ArgumentException("An item needs some text.", nameof(text));
        }

        if (trimmed.Length > MaxLength)
        {
            throw new ArgumentException($"An item can be at most {MaxLength} characters.", nameof(text));
        }

        return new ItemText(trimmed);
    }

    public override string ToString() => Value;
}
