namespace MentalInventory.Domain.Tests.Inventories;

// Every rule is a "given these events, when this command, then these events" test.
public class InventoryDeciderTests
{
    private static readonly InventoryId Id = InventoryId.New();
    private static readonly ItemId First = ItemId.New();
    private static readonly ItemId Second = ItemId.New();
    private static readonly DateTimeOffset Morning = new(2026, 10, 10, 8, 0, 0, TimeSpan.FromHours(2));
    private static readonly DateTimeOffset Noon = Morning.AddHours(4);

    private static readonly InventoryStarted Started = new(Id, Morning);
    private static readonly ItemAdded FirstAdded = new(First, Column.WorkingOn, "Write the domain model");
    private static readonly ItemAdded SecondAdded = new(Second, Column.WantToBeWorkingOn, "Learn the cello");

    private static IReadOnlyList<IInventoryEvent> When(IInventoryCommand command, params IInventoryEvent[] given) =>
        InventoryDecider.Decide(command, InventoryDecider.Fold(given));

    private static void Then(IReadOnlyList<IInventoryEvent> actual, params IInventoryEvent[] expected) =>
        Assert.Equal(expected, actual);

    private static void Rejected(IInventoryCommand command, params IInventoryEvent[] given) =>
        Assert.Throws<InventoryRuleViolation>(() => When(command, given));

    [Fact]
    public void Starting_a_new_inventory_records_when()
    {
        Then(When(new StartInventory(Id, Morning)), Started);
    }

    [Fact]
    public void An_inventory_can_only_be_started_once()
    {
        Rejected(new StartInventory(Id, Noon), Started);
    }

    [Fact]
    public void Nothing_can_be_added_before_the_inventory_is_started()
    {
        Rejected(new AddItem(First, Column.WorkingOn, ItemText.From("Too early")));
    }

    [Fact]
    public void Adding_an_item_stores_the_trimmed_text()
    {
        var events = When(new AddItem(First, Column.WorkingOn, ItemText.From("  Write the domain model ")), Started);

        Then(events, FirstAdded);
    }

    [Fact]
    public void An_item_id_cannot_be_reused()
    {
        Rejected(new AddItem(First, Column.ShouldBeWorkingOn, ItemText.From("Again")), Started, FirstAdded);
    }

    [Fact]
    public void Moving_an_item_records_the_new_column()
    {
        var events = When(new MoveItem(First, Column.ShouldBeWorkingOn), Started, FirstAdded);

        Then(events, new ItemMoved(First, Column.ShouldBeWorkingOn));
    }

    [Fact]
    public void Moving_an_item_to_its_own_column_changes_nothing()
    {
        Assert.Empty(When(new MoveItem(First, Column.WorkingOn), Started, FirstAdded));
    }

    [Fact]
    public void A_moved_item_ends_up_last_in_its_new_column()
    {
        var third = new ItemAdded(ItemId.New(), Column.ShouldBeWorkingOn, "File taxes");

        var state = InventoryDecider.Fold([Started, FirstAdded, third, new ItemMoved(First, Column.ShouldBeWorkingOn)]);

        Assert.Equal(new[] { third.ItemId, First }, state.InColumn(Column.ShouldBeWorkingOn).Select(item => item.Id));
    }

    [Fact]
    public void Rewording_to_the_same_text_changes_nothing()
    {
        Assert.Empty(When(new RewordItem(First, ItemText.From("Write the domain model")), Started, FirstAdded));
    }

    [Fact]
    public void Rewording_an_unknown_item_is_rejected()
    {
        Rejected(new RewordItem(Second, ItemText.From("Who?")), Started, FirstAdded);
    }

    [Fact]
    public void A_removed_item_is_gone()
    {
        var events = When(new RemoveItem(First), Started, FirstAdded);
        var state = InventoryDecider.Fold([Started, FirstAdded, .. events]);

        Then(events, new ItemRemoved(First));
        Assert.Empty(state.Items);
    }

    [Fact]
    public void An_empty_inventory_cannot_be_reviewed()
    {
        Rejected(new StartReview(), Started);
    }

    [Fact]
    public void Items_cannot_be_added_during_review()
    {
        Rejected(new AddItem(Second, Column.WorkingOn, ItemText.From("Late")), Started, FirstAdded, new ReviewStarted());
    }

    [Fact]
    public void Items_cannot_be_reviewed_while_listing()
    {
        Rejected(new ReviewItem(First, Verdict.Vital), Started, FirstAdded);
    }

    [Fact]
    public void Reviewing_an_item_records_the_verdict()
    {
        var events = When(new ReviewItem(First, Verdict.Struck), Started, FirstAdded, new ReviewStarted());

        Then(events, new ItemReviewed(First, Verdict.Struck));
    }

    [Fact]
    public void A_verdict_can_be_changed_but_repeating_it_changes_nothing()
    {
        IInventoryEvent[] given = [Started, FirstAdded, new ReviewStarted(), new ItemReviewed(First, Verdict.Struck)];

        Assert.Empty(When(new ReviewItem(First, Verdict.Struck), given));
        Then(When(new ReviewItem(First, Verdict.Matters), given), new ItemReviewed(First, Verdict.Matters));
    }

    [Fact]
    public void An_inventory_with_unreviewed_items_cannot_be_completed()
    {
        Rejected(new CompleteInventory(Noon), Started, FirstAdded, SecondAdded, new ReviewStarted(), new ItemReviewed(First, Verdict.Vital));
    }

    [Fact]
    public void Completing_records_when()
    {
        var events = When(new CompleteInventory(Noon), Started, FirstAdded, new ReviewStarted(), new ItemReviewed(First, Verdict.Vital));

        Then(events, new InventoryCompleted(Noon));
    }

    [Fact]
    public void A_completed_inventory_is_read_only()
    {
        IInventoryEvent[] completed =
            [Started, FirstAdded, new ReviewStarted(), new ItemReviewed(First, Verdict.Vital), new InventoryCompleted(Noon)];
        IInventoryCommand[] anyCommand =
        [
            new StartInventory(Id, Noon),
            new AddItem(ItemId.New(), Column.WorkingOn, ItemText.From("More")),
            new RewordItem(First, ItemText.From("Changed")),
            new MoveItem(First, Column.WantToBeWorkingOn),
            new RemoveItem(First),
            new StartReview(),
            new ReviewItem(First, Verdict.Struck),
            new CompleteInventory(Noon),
        ];

        Assert.All(anyCommand, command => Rejected(command, completed));
    }

    [Fact]
    public void Folding_a_whole_inventory_gives_the_kept_items()
    {
        var state = InventoryDecider.Fold(
        [
            Started,
            FirstAdded,
            SecondAdded,
            new ReviewStarted(),
            new ItemReviewed(First, Verdict.Vital),
            new ItemReviewed(Second, Verdict.Struck),
            new InventoryCompleted(Noon),
        ]);

        Assert.Equal(Id, state.Id);
        Assert.Equal(Phase.Completed, state.Phase);
        Assert.Equal(Morning, state.StartedAt);
        Assert.Equal(Noon, state.CompletedAt);
        Assert.Equal(new[] { First }, state.Kept.Select(item => item.Id));
    }
}
