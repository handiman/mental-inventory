# 0004. Event-sourced domain model with the decider pattern

Date: 2026-10-10

## Status

Proposed

## Context

The domain is small: an inventory is started, items are written into three columns, each
item is reviewed ("is it vital, does it matter?") and the inventory is completed. See
[docs/domain-model.md](../domain-model.md).

What shapes the choice of model is not the size of the domain but where it runs:

- Today the app is a standalone WebAssembly app (ADR-0002), so all data lives in the
  user's browser and the app must work offline.
- Later there will be a MAUI app and possibly a Cloudflare Worker backend, so the same
  user's data will be edited on more than one device, sometimes offline, and must sync.
- The project is also meant to show how the author builds software, so the domain should
  be clean and its rules easy to read and test.

Options considered:

- **State-based (CRUD):** store each inventory as a document holding its current state.
  The simplest option. Completed inventories are immutable, so history *across*
  inventories ("this has been in 'should' for three months") comes for free. But changes
  *within* an inventory are lost, and syncing two offline edits of the same document means
  last-write-wins, which silently drops one side.
- **Event sourcing with a classic aggregate:** a mutable aggregate class with `Apply`
  methods and a list of uncommitted events, usually on a framework base class. Familiar,
  but it mixes state mutation, rules and event plumbing in one class.
- **Event sourcing with the decider pattern:** behaviour as two pure functions over
  immutable records: `Decide(command, state) -> events` and `Evolve(state, event) -> state`.
  State is a left fold of the events.

## Decision

Event sourcing with the decider pattern, one event stream per inventory, in a
`MentalInventory.Domain` project with no package or framework references.

Event sourcing over CRUD, because:

- Sync becomes merging append-only facts. Items added on two devices both survive;
  genuine conflicts (two verdicts on the same item) are rare and visible as events.
- The change history within an inventory (what was struck, what moved from "want" to
  "should") is kept, which leaves room for reflection features later. CRUD would throw it
  away for good.
- Undo is folding one event fewer.

The decider over a classic aggregate, because:

- Rules are pure functions, so every rule is a test of the form "given these events, when
  this command, then these events", with no mocks, clock or storage.
- State is an immutable record that Blazor can render directly. There is no base class and
  no event-store library.
- Commands that would change nothing (moving an item to the column it is already in)
  produce no events, so the stream stays free of noise.

One stream per inventory, because an inventory is a short session with a clear end.
Streams stay small, replay is cheap even in WebAssembly, and a completed stream never
changes again, so it can be snapshotted or turned into a read model once.

## Consequences

- Events are a public schema that we cannot migrate centrally: once they are stored in a
  user's browser, old events must replay forever. So events are never changed, only added
  (or versioned and upcast on read); enum values are explicit and never renumbered; and
  events carry plain values (item text as `string`, not the validated `ItemText`) so that
  tightening a validation rule later never breaks replay.
- Persistence, serialization and the mapping of event types to stable names are not
  decided here. That belongs in the persistence work (likely IndexedDB in the browser) and
  its own ADR.
- Read models are needed for anything across inventories (lists, history, statistics).
  With one stream per inventory these are simple projections of completed streams.
- If multi-device sync never happens, this is more machinery than the domain strictly
  needs. The change history and the testability still pay for part of it, and switching a
  domain this small to CRUD later would be cheap.
