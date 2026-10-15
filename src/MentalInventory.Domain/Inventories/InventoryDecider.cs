namespace MentalInventory.Domain.Inventories;

/// <summary>
/// The behaviour of an inventory, as pure functions (the "decider" pattern):
/// <list type="bullet">
/// <item><see cref="Decide"/>: given the current state, what happens if we do this?</item>
/// <item><see cref="Evolve"/>: given the current state, what is the state after this event?</item>
/// </list>
/// No I/O, no clock, no mutable state, so every rule is tested as given events, when
/// command, then events. See docs/adr/0004.
/// </summary>
public static class InventoryDecider
{
    public static Inventory Initial => Inventory.Initial;

    /// <summary>
    /// Decides which events a command results in. Returns no events when the command would
    /// change nothing (moving an item to the column it is in, for example).
    /// </summary>
    /// <exception cref="InventoryRuleViolation">The command breaks a rule.</exception>
    public static IReadOnlyList<IInventoryEvent> Decide(IInventoryCommand command, Inventory state) =>
        command switch
        {
            StartInventory c => Start(c, state),
            AddItem c => Add(c, state),
            RewordItem c => Reword(c, state),
            MoveItem c => Move(c, state),
            RemoveItem c => Remove(c, state),
            StartReview => BeginReview(state),
            ReviewItem c => Review(c, state),
            CompleteInventory c => Complete(c, state),
            _ => throw new ArgumentOutOfRangeException(nameof(command), command, "Unknown command."),
        };

    /// <summary>Applies an event. Events are facts, so nothing is validated here.</summary>
    public static Inventory Evolve(Inventory state, IInventoryEvent @event) =>
        @event switch
        {
            InventoryStarted e => state with { Id = e.InventoryId, Phase = Phase.Listing, StartedAt = e.StartedAt },
            ItemAdded e => state with { Items = state.Items.Add(new Item(e.ItemId, e.Column, e.Text)) },
            ItemReworded e => Update(state, e.ItemId, item => item with { Text = e.Text }),
            // Remove and re-add, so a moved item ends up last in its new column.
            ItemMoved e => state.Find(e.ItemId) is { } moved
                ? state with { Items = state.Items.Remove(moved).Add(moved with { Column = e.To }) }
                : state,
            ItemRemoved e => state with { Items = state.Items.RemoveAll(item => item.Id == e.ItemId) },
            ReviewStarted => state with { Phase = Phase.Reviewing },
            ItemReviewed e => Update(state, e.ItemId, item => item with { Verdict = e.Verdict }),
            InventoryCompleted e => state with { Phase = Phase.Completed, CompletedAt = e.CompletedAt },
            _ => throw new ArgumentOutOfRangeException(nameof(@event), @event, "Unknown event."),
        };

    /// <summary>Rebuilds the state from an event stream.</summary>
    public static Inventory Fold(IEnumerable<IInventoryEvent> events) => events.Aggregate(Initial, Evolve);

    private static IInventoryEvent[] Start(StartInventory command, Inventory state)
    {
        if (state.Phase != Phase.NotStarted)
        {
            throw new InventoryRuleViolation("The inventory has already been started.");
        }

        return [new InventoryStarted(command.InventoryId, command.Now)];
    }

    private static IInventoryEvent[] Add(AddItem command, Inventory state)
    {
        RequirePhase(state, Phase.Listing, "Items can only be added while listing.");
        if (state.Find(command.ItemId) is not null)
        {
            throw new InventoryRuleViolation($"Item {command.ItemId} already exists.");
        }

        return [new ItemAdded(command.ItemId, command.Column, command.Text.Value)];
    }

    private static IInventoryEvent[] Reword(RewordItem command, Inventory state)
    {
        RequirePhase(state, Phase.Listing, "Items can only be reworded while listing.");
        var item = RequireItem(state, command.ItemId);
        return item.Text == command.Text.Value ? [] : [new ItemReworded(item.Id, command.Text.Value)];
    }

    private static IInventoryEvent[] Move(MoveItem command, Inventory state)
    {
        RequirePhase(state, Phase.Listing, "Items can only be moved while listing.");
        var item = RequireItem(state, command.ItemId);
        return item.Column == command.To ? [] : [new ItemMoved(item.Id, command.To)];
    }

    private static IInventoryEvent[] Remove(RemoveItem command, Inventory state)
    {
        RequirePhase(state, Phase.Listing, "Items can only be removed while listing.");
        var item = RequireItem(state, command.ItemId);
        return [new ItemRemoved(item.Id)];
    }

    private static IInventoryEvent[] BeginReview(Inventory state)
    {
        RequirePhase(state, Phase.Listing, "Review can only start from listing.");
        if (state.Items.IsEmpty)
        {
            throw new InventoryRuleViolation("There is nothing to review yet.");
        }

        return [new ReviewStarted()];
    }

    private static IInventoryEvent[] Review(ReviewItem command, Inventory state)
    {
        RequirePhase(state, Phase.Reviewing, "Items can only be reviewed during review.");
        var item = RequireItem(state, command.ItemId);
        return item.Verdict == command.Verdict ? [] : [new ItemReviewed(item.Id, command.Verdict)];
    }

    private static IInventoryEvent[] Complete(CompleteInventory command, Inventory state)
    {
        RequirePhase(state, Phase.Reviewing, "Only an inventory under review can be completed.");
        if (!state.IsFullyReviewed)
        {
            throw new InventoryRuleViolation("Every item needs a verdict before the inventory is complete.");
        }

        return [new InventoryCompleted(command.Now)];
    }

    private static void RequirePhase(Inventory state, Phase phase, string message)
    {
        if (state.Phase != phase)
        {
            throw new InventoryRuleViolation(message);
        }
    }

    private static Item RequireItem(Inventory state, ItemId id) =>
        state.Find(id) ?? throw new InventoryRuleViolation($"Item {id} does not exist.");

    private static Inventory Update(Inventory state, ItemId id, Func<Item, Item> change) =>
        state with { Items = state.Items.ConvertAll(item => item.Id == id ? change(item) : item) };
}
