# 0003. Deploy with Cloudflare Workers Builds (Git integration)

Date: 2026-10-10

## Status

Accepted

## Context

The web app is a static Blazor WebAssembly build served by a static-assets-only Cloudflare
Worker (see [ADR-0002](0002-blazor-webassembly-standalone-with-shared-rcl.md) and
`wrangler.toml`). It needs a deploy pipeline that fits the PR-based workflow in
CONTRIBUTING.md: `master` deploys to production, and every other branch should be previewable.

Options considered:

- **Manual `wrangler deploy`:** simple, but easy to forget and not tied to `master`.
- **GitHub Actions + `wrangler`:** what dopp-site uses. Full control and a cached .NET
  SDK, but it needs an API token as a repository secret and a workflow to maintain.
- **Cloudflare Workers Builds:** Cloudflare builds and deploys on push, with no workflow
  file or deploy token in GitHub. Non-production branches get preview versions. The build
  image has no .NET SDK.

## Decision

Use Workers Builds, connected to this repository:

- Production branch: `master`, deployed with `npx wrangler deploy`.
- Other branches: `npx wrangler versions upload`, so each branch gets a preview version.
- Build command: install the .NET 10 SDK with Microsoft's `dotnet-install.sh`, then
  `~/.dotnet/dotnet publish src/MentalInventory.Web -c Release`.

The Worker name in the dashboard must match `name` in `wrangler.toml`.

## Consequences

- No deploy secrets or workflow files in the repository; merging a PR to `master` is the
  release.
- The SDK is downloaded on every build, adding a minute or two.
- Build configuration lives in the Cloudflare dashboard, not in the repository, so changes
  to it are not reviewed in PRs. Document any change here or in CLAUDE.md.
- Tests do not run in this pipeline. They are gated before merge instead: the GitHub Actions
  workflow `.github/workflows/ci.yml` runs `dotnet build` and `dotnet test` on every PR, and
  a ruleset on `master` requires a pull request, requires the `build-and-test` status check
  to pass and blocks force pushes. Only code that passed CI reaches `master`, and so
  production.
- If the build image or SDK download becomes a problem, GitHub Actions + `wrangler` is the
  fallback.
