namespace MentalInventory.Domain.Tests.Inventories;

public class ItemTextTests
{
    [Fact]
    public void Text_is_trimmed()
    {
        Assert.Equal("Call mum", ItemText.From("  Call mum\t").Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Text_cannot_be_empty(string? text)
    {
        Assert.Throws<ArgumentException>(() => ItemText.From(text));
    }

    [Fact]
    public void Text_has_a_maximum_length()
    {
        Assert.Equal(ItemText.MaxLength, ItemText.From(new string('x', ItemText.MaxLength)).Value.Length);
        Assert.Throws<ArgumentException>(() => ItemText.From(new string('x', ItemText.MaxLength + 1)));
    }
}
