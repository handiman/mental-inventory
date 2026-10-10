# Architectural Decision Records

An ADR captures one significant decision: the problem, the options weighed, what was
chosen and the consequences. The point is the *reasoning*, so a later reader (human or
Claude Code) can tell whether it still holds before changing course.

## When to write one

For a decision that is costly to reverse, affects more than one part of the app, or picks
between two reasonable options for a non-obvious reason: a new dependency, a data model, a
cross-cutting pattern, "we considered X but chose Y because Z". Not for things with an
obvious answer.

## Rules

- [CLAUDE.md](../../CLAUDE.md) describes how things work *now*. An ADR is a frozen record
  and is not edited after acceptance. To change a decision, write a new ADR and set the old
  one's status to `Superseded by ADR-NNNN`.
- Files are named `NNNN-kebab-case-title.md`. Numbers are never reused. Start from
  [template.md](template.md).
- Status: **Proposed**, **Accepted**, **Deprecated** or **Superseded by ADR-NNNN**.

## Index

| # | Title | Status |
|---|-------|--------|
| [0001](0001-record-architecture-decisions.md) | Record architectural decisions with ADRs | Accepted |
| [0002](0002-blazor-webassembly-standalone-with-shared-rcl.md) | Blazor WebAssembly standalone with a shared Razor Class Library, MAUI Blazor Hybrid later | Accepted |
