# Mental Inventory

A small app for the first step of the Bullet Journal method: the *mental inventory*.
Following Ryder Carroll, you write down everything on your mind in three columns:

- Things I'm working on
- Things I should be working on
- Things I want to be working on

## Stack

- **UI:** Blazor, with components in a shared Razor Class Library
- **Web:** Blazor WebAssembly (standalone, PWA), hosted as static files on Cloudflare
- **Mobile/desktop (later):** .NET MAUI Blazor Hybrid, reusing the same components

## Structure

```
src/MentalInventory.Shared   Components, pages, layout, routing
src/MentalInventory.Web      Blazor WebAssembly host
test/                        Tests
```

## Getting started

Requires the .NET 10 SDK.

```bash
dotnet build
dotnet test
dotnet run --project src/MentalInventory.Web
```

## License

[MIT](LICENSE)
