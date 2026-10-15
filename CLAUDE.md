# CLAUDE.md

Guidance for Claude Code in this repository.

## What this is

Mental Inventory: a Blazor app for the Bullet Journal "mental inventory" step (three
columns: working on / should be working on / want to be working on). Web first; a
.NET MAUI Blazor Hybrid app is planned later. Don't add a MAUI project unless asked.

## Structure

- `src/MentalInventory.Domain` (class library): the domain model. Event sourced with the
  decider pattern: pure `Decide`/`Evolve` functions over immutable records, one stream per
  inventory. No package, UI, persistence or serialization references. Never change or
  remove a field on an existing event. See [docs/domain-model.md](docs/domain-model.md) and
  [ADR-0004](docs/adr/0004-event-sourced-domain-with-the-decider-pattern.md).
- `src/MentalInventory.Shared` (Razor Class Library): all UI, including the components,
  pages, layout and `Routes.razor`. New UI goes here, never in a host project.
- `src/MentalInventory.Web` (Blazor WebAssembly Standalone, PWA): a thin host. It only
  holds `Program.cs`, `index.html` and the PWA files.
- `test/`: tests, one project per tested project.
- `Directory.Build.props` sets `TargetFramework`, `Nullable`, `ImplicitUsings` and
  `TreatWarningsAsErrors`. Don't repeat them in project files.

## Architectural Decision Records

Significant decisions are recorded in [docs/adr/](docs/adr/). This file describes current
state; an ADR is the frozen *why*. Check it before relitigating a decision, and add a new
one for a new significant decision. See [docs/adr/README.md](docs/adr/README.md).

## Hosting constraint

The web app is deployed as static files to Cloudflare. It must stay a *standalone*
WebAssembly app. Don't introduce anything that needs an ASP.NET Core server at runtime
(Blazor Web App template, server/auto render modes, prerendering). A backend, if any,
will be a Cloudflare Worker in TypeScript. See
[ADR-0002](docs/adr/0002-blazor-webassembly-standalone-with-shared-rcl.md).

## Observability

Instrument against the BCL, never a telemetry SDK (see
[ADR-0005](docs/adr/0005-observability-on-bcl-diagnostics-without-an-exporter-yet.md)):

- Traces: `Telemetry.ActivitySource.StartActivity(...)` and use `?.` on the result.
  Metrics: instruments created from `Telemetry.Meter`. Don't create other sources or meters.
- Logs: add a `[LoggerMessage]` method to `Diagnostics/Log.cs` (or a partial in the class
  that logs) with a new, unique event id. No string-interpolated log calls.
- Never put item texts or other user content in logs, tags or metrics.

## Deployment

Cloudflare Workers Builds deploys `master` to production on every push; other branches get
preview versions. Build settings live in the Cloudflare dashboard, see
[ADR-0003](docs/adr/0003-deploy-with-cloudflare-workers-builds.md). `wrangler.toml` must
keep `name = "mental-inventory"` and the assets directory pointing at the publish output.

## Commands

```bash
dotnet build                                   # must be clean; warnings are errors
dotnet test
dotnet run --project src/MentalInventory.Web
dotnet publish src/MentalInventory.Web -c Release   # static output in bin/Release/net10.0/publish/wwwroot
```

## Git

Branching and commit format: see [CONTRIBUTING.md](CONTRIBUTING.md). Add the
`Co-Authored-By` trailer when committing as Claude Code.

- Never commit or push directly to `master`. Every change, however small, goes on its own
  short-lived branch and through a PR.
- Pushing a branch and opening a PR needs no confirmation.
- Never merge into `master`. That is always the user's step.
- Never rebase or force-push a pushed branch. Merge `origin/master` into it instead.
