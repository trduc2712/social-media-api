# Secrets

This file lists the settings the API needs that are never committed. It records their names and where each value comes from, never the values themselves.

## Storage

| Setting | Value |
| --- | --- |
| `Storage:ServiceUrl` | The S3 endpoint of the bucket, `https://s3.<region>.backblazeb2.com` |
| `Storage:Region` | The bucket region, for example `eu-central-003` |
| `Storage:BucketName` | The bucket name |
| `Storage:AccessKeyId` | The `keyID` of the Backblaze application key |
| `Storage:SecretAccessKey` | The `applicationKey` of the Backblaze application key |

The endpoint and region are shown on the bucket page in Backblaze. The `applicationKey` is shown only once, when the application key is created.

The API refuses to start while any of these is missing.

## Local development

Local values are kept with .NET user secrets, in a file under your home directory, outside the repository. They are loaded automatically when the API runs in the Development environment.

Set a secret:

```bash
make secret NAME=Storage:BucketName VALUE=<value>
```

Show the secrets that are set, with their values:

```bash
make secrets
```

## Staging and production

Set the same settings as environment variables on the host, with `__` in place of `:`, for example `Storage__BucketName`.
