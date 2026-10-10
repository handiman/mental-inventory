# 0001. Record architectural decisions with ADRs

Date: 2026-10-10

## Status

Accepted

## Context

CLAUDE.md is a living document that is edited as the repository changes, so the reasoning
behind a decision and the alternatives considered tend to get trimmed away over time. The
same convention is already used in the Dopp repositories.

## Decision

Record significant decisions as ADRs in `docs/adr/`, using a short
Context/Decision/Consequences format. CLAUDE.md describes current state and links to the
relevant ADRs.

## Consequences

The reasoning behind decisions stays available. The cost is a little process overhead, and
a superseded ADR's status must be updated when a new one replaces it.
