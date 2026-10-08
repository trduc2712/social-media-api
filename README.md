# Social Media API

The backend API for a social media web app.

## Staging

Every push to `main` runs the tests, applies pending database migrations to the staging database, and then deploys to Render.

## Database documentation

The schema is documented in [docs/database](docs/database/README.md). It is generated from the migrated database with [tbls](https://github.com/k1LoW/tbls), so do not edit it by hand. After adding a migration, run `make migrate` and then `make db-docs`, and commit the result. CI fails if the docs are out of date or a table or column has no description.
