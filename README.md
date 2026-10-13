# Social Media API

The backend API for a social media web app.

## Staging

Every push to `main` runs the tests, applies pending database migrations to the staging database, and then deploys to Render.

## Database documentation

The schema is documented in [docs/database](docs/database/README.md). It is generated from the migrated database with [tbls](https://github.com/k1LoW/tbls), so do not edit it by hand. After adding a migration, run `make migrate` and then `make db-docs`, and commit the result. CI fails if the docs are out of date or a table or column has no description.

## Storage

Files are kept in a private S3-compatible bucket (Backblaze B2 in staging and production). Clients upload and download directly with short-lived presigned URLs issued by the API, so no file passes through it.

No storage setting is kept in the repository, in any environment, and the API refuses to start while one is missing. [docs/secrets.md](docs/secrets.md) lists the settings and how to provide them: .NET user secrets locally, environment variables in staging and production.

`Storage:ForcePathStyle` is only needed for providers that do not support virtual-hosted bucket addresses.

The bucket needs a CORS rule that allows `PUT` and `GET` with the `Content-Type` header from the web app origins, otherwise browsers cannot use the presigned URLs.

The storage integration tests start their own MinIO container, so `make test` needs a running Docker daemon.
