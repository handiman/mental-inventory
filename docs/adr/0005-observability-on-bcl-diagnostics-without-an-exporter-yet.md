# 0005. Observability on BCL diagnostics, without an exporter yet

Date: 2026-10-10

## Status

Proposed

## Context

The app is a standalone Blazor WebAssembly app with no backend (ADR-0002, ADR-0003).
There is nowhere to send telemetry yet, but code written now should not have to be
revisited to become observable later, when there is a Cloudflare Worker backend and a
MAUI app.

Options considered:

- **Nothing until needed:** cheapest now, but instrumentation added after the fact tends
  to be patchy, and log calls written ad hoc are hard to make structured later.
- **OpenTelemetry .NET SDK with an OTLP exporter in the browser:** the standard, but the
  SDK is built for server processes. Its batching exporter relies on background threads,
  which Blazor WebAssembly doesn't have by default, it adds download size to every visit,
  and there is no collector to send to.
- **BCL diagnostics only:** `ActivitySource` for traces, `Meter` for metrics and `ILogger`
  for logs. These are the APIs OpenTelemetry .NET itself reads, they ship with the runtime,
  and they cost next to nothing when nobody listens.

## Decision

Instrument against the BCL APIs, and add no SDK or exporter yet:

- One `ActivitySource` and one `Meter`, both named `MentalInventory`, in
  `MentalInventory.Shared.Diagnostics.Telemetry`. No other sources or meters.
- Logs through source-generated `[LoggerMessage]` methods with stable event ids, in the
  `Log` class.
- In development, `ActivityLogListener` writes finished activities to the log, so traces
  are visible in the browser console.
- Log levels are set in `Program.cs`: our categories at Debug in development and
  Information otherwise; `Microsoft.*` and `System.*` at Warning.

## Consequences

- Call sites are final. Adding an exporter later is a host concern: subscribe to the
  `MentalInventory` source and meter, in the Worker or in a small browser-side exporter.
- Telemetry stays in the browser for now: no production visibility until an exporter
  exists. Unhandled errors only reach the user's console.
- The choice of exporter and backend (Workers observability, an OTLP endpoint on the
  Worker, a third-party service) needs its own ADR, and must weigh privacy: item texts are
  personal and must never end up in telemetry.
