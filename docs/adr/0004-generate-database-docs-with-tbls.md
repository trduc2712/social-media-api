# 4. Generate Database Documentation with tbls

Date: 2026-10-08

## Status

Accepted

## Context

We want documentation of the database schema: the tables, their columns,
keys and indexes, how they relate, and what each of them means.

The schema is defined by EF Core migrations and nowhere else. Any document
written by hand, such as a DBML file or a Mermaid diagram, is a second
description of the same thing. Nothing forces it to be updated when a
migration is added, so sooner or later it is wrong, and wrong documentation
is worse than none.

Generating the documentation from the EF Core model would miss whatever a
migration does in raw SQL. Generating it from a migrated database describes
what really exists.

## Decision

We generate the documentation from a migrated PostgreSQL database with
[tbls](https://github.com/k1LoW/tbls) and commit the result.

- The output lives in `docs/database/`: a `README.md` with the list of tables
  and an ER diagram, and one Markdown page per table. Diagrams are Mermaid,
  which GitHub renders. Nobody edits these files by hand.
- Descriptions are written once, in code, with `HasComment(...)` in the EF
  Core configuration of each entity. The migration turns them into PostgreSQL
  comments and tbls reads them from the database.
- `make db-docs` regenerates the documentation, `make db-docs-check` fails if
  the committed files differ from the database, and `make db-docs-lint` fails
  if a table or column has no description.
- tbls is configured in `.tbls.yml`. Its version is pinned in the `Makefile`
  and it is run with `go run`, so there is nothing to install besides Go.
- The `db-docs` job in `ci.yml` starts an empty PostgreSQL, applies all
  migrations, then runs the check and the lint. The staging migration waits
  for it.

## Consequences

Easier:

- The documentation cannot drift from the schema without CI failing.
- A schema change shows up as a readable Markdown diff in the pull request,
  next to the migration that caused it.
- The descriptions are also in the database itself, visible from `psql` or
  any database client.

Harder:

- A table cannot be documented before its migration exists. Design
  discussions that happen earlier need another place, such as an ADR.
- Regenerating needs Go and a local database with all migrations applied.
  The first run compiles tbls, which takes a couple of minutes.
- Every migration now comes with a second step, `make db-docs`, and CI
  rejects the push if it is forgotten.
- Changing a description means adding a migration, because the description
  is part of the schema.
- CI has one more job, and it needs a PostgreSQL service container whose
  major version should follow the one we run.
