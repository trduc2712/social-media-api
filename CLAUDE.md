# CLAUDE.md

## Commands

- Build: `make build`
- Test: `make test` (by type: `make test TYPE=unit|integration|functional|architecture`,
  filtered: `make test FILTER=<Name>`)
- Format: `make format`
- Add migration: `make migration NAME=<Name>`
- Run API: `make run` (with hot reload: `make watch`)
- Database docs: `make db-docs` (verify: `make db-docs-check`, `make db-docs-lint`)

Use these instead of raw `dotnet` commands. Run `make build` and the relevant
tests before saying you're done.

## Architecture

- This .NET project using Clean Architecture.
- Domain and Contract depend on nothing, not even each other. Application
  depends only on Domain and Contract. Infrastructure depends on Application.
  Api depends on Application, Contract and Infrastructure. Never reference
  Infrastructure or Api from Application, Domain or Contract.
- Test projects live in `tests/`. Tests.Common references only Domain. Each
  `<Layer>.UnitTests`, `IntegrationTests` or `FunctionalTests` project
  references its own layer plus Tests.Common. ArchitectureTests references all
  five production projects and enforces the rules above.
- One use case = one MediatR command or query + handler + validator + tests.
- Controllers only dispatch to MediatR. No logic in controllers.

## Rules

- Don't write comments unless asked.
- Use the standardized Makefile commands instead of running CLI commands manually.
- Queries return DTOs, never EF entities.
- Pass CancellationToken through every async call.
- Describe every table and column with `HasComment(...)` in its EF
  configuration. After adding a migration, run `make migrate` and
  `make db-docs` in the same change. Never edit `docs/database/` by hand.

## Boundaries

- Never commit, push, or open a PR.
- Never edit an existing migration. Add a new one.
