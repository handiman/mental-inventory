# Contributing

## Branching

`master` is the trunk and always deployable. Never push directly to `master`.

All work happens on small, short-lived branches named `<type>/<kebab-case-summary>` (same
types as commits), e.g. `feat/inventory-columns`, and reaches `master` through a pull
request. Nothing is too small for a PR. Merge back small and often.

Never rebase or force-push a branch that has been pushed. To pick up changes from `master`,
merge it into the branch (`git merge origin/master`) instead.

## Commit messages

Subjects follow [Conventional Commits](https://www.conventionalcommits.org/):
`<type>: <Summary>`, e.g. `feat: Add the three inventory columns`.

Common types: `feat` (user-visible capability), `fix`, `docs`, `refactor` (no behavior
change), `test`, `chore` (tooling, config, dependencies), `perf`, `ci`.

## Before pushing

`dotnet build` (warnings are errors) and `dotnet test` must both pass.
