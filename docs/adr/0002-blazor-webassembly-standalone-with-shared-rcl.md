# 0002. Blazor WebAssembly standalone with a shared Razor Class Library, MAUI Blazor Hybrid later

Date: 2026-10-10

## Status

Accepted

## Context

The project is meant to be useful in itself, to showcase .NET/C# skills, and possibly to
generate income later. That argues for a C# UI rather than Flutter (used for Dopp). The
backend will run on Cloudflare for cost reasons, which means TypeScript Workers, since
Workers do not run .NET. Google Play is not a near-term option for an indie developer, so
web comes first.

Options considered:

- **Avalonia UI:** closest to Flutter technically (own Skia rendering, all targets
  including WebAssembly), but less in demand on the consulting market.
- **Uno Platform:** similar reach with a WinUI API; parts of its tooling are paid.
- **.NET MAUI alone:** no web target.
- **Blazor Web App template:** requires an ASP.NET Core server, which cannot run on
  Cloudflare.
- **Blazor WebAssembly standalone + MAUI Blazor Hybrid:** one set of Razor components for
  web and native, and Blazor is the most requested of these skills.

## Decision

Use Blazor. All UI lives in `MentalInventory.Shared`, a Razor Class Library.
`MentalInventory.Web` is a standalone Blazor WebAssembly host (PWA), published as static
files to Cloudflare. A .NET MAUI Blazor Hybrid host is added when native apps become
relevant.

## Consequences

- The web app downloads the .NET runtime, so the first load is a few MB and slower than a
  JavaScript or Flutter web app. The PWA service worker caches it for later visits.
- Nothing that needs a server at runtime can be used: no server or auto render modes, no
  prerendering.
- Native apps can reuse the components without a rewrite, but MAUI-specific concerns
  (platform APIs, store packaging) remain future work.
