# 3. Apply Database Migrations in CI

Date: 2026-10-08

## Status

Accepted

## Context

Schema changes are EF Core migrations, and the staging database must have
them applied before the new version of the API starts serving requests.

The common shortcut is to call `Database.Migrate()` when the app starts.
That has drawbacks we want to avoid:

- The app's database account needs permission to change the schema, not just
  to read and write data.
- If several instances start at once, they race to apply the same
  migrations.
- A failing migration surfaces as an app that will not start, after the
  deploy has already begun.

Running `dotnet ef database update` from a pipeline avoids those, but needs
the source code and the .NET SDK wherever it runs.

## Decision

We apply migrations from CI, as a separate step that runs before the deploy.
The app never migrates its own database.

- `make migrations-bundle` builds an EF Core migration bundle, a
  self-contained executable holding all migrations, at `artifacts/efbundle`.
- `.github/workflows/migrate-database.yml` is a reusable workflow that builds
  the bundle and runs it against the database of the environment it is
  called for. The connection string is passed as a secret.
- `ci.yml` calls it for staging on every push to `main`, only after the
  `test` job has passed.
- Runs for the same environment are queued, never cancelled or run in
  parallel, so two pushes cannot migrate the same database at once.
- The API is deployed to Render only after the migration job has succeeded.

## Consequences

Easier:

- A failing migration stops the pipeline, and the version that is already
  running stays up.
- The app's runtime image contains no migration tooling, and the app can run
  with a database account that cannot change the schema.
- Adding another environment, such as production, means calling the same
  workflow with a different name and secret.

Harder:

- For a short time the old version of the API runs against the new schema.
  Every migration must therefore be backward compatible with the version
  before it. A breaking change, such as dropping or renaming a column, has to
  be split across two releases.
- There is no automatic rollback. If the deploy fails after the migration
  succeeded, the schema stays ahead of the code until we fix forward or apply
  a reverting migration.
- The CI runner needs network access to the database and holds its
  connection string as a GitHub secret.
- The pipeline has one more job that can fail, and it rebuilds the bundle on
  every push to `main`, even when there are no new migrations.
