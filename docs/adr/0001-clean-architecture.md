# 1. Use Clean Architecture

Date: 2026-10-08

## Status

Accepted

## Context

This is the backend API for a social media web app. It will grow one feature
at a time, and it depends on external pieces that should stay replaceable and
out of the business rules: PostgreSQL through EF Core and Npgsql, and Clerk
for identity.

The code base is maintained by one person, so structural rules need to be
checked mechanically rather than by code review.

## Decision

We split the solution into five production projects with fixed dependency
directions:

- `Domain` and `Contract` reference nothing, not even each other.
- `Application` references only `Domain` and `Contract`.
- `Infrastructure` references `Application`.
- `Api` references `Application`, `Contract` and `Infrastructure`.

We model each use case as one MediatR command or query with its handler,
FluentValidation validator and tests, in its own folder, for example
`SocialMedia.Application/Users/GetCurrentUser/`.

Controllers only dispatch to MediatR through `ISender` and hold no logic.
Queries return `Contract` DTOs, never EF entities.

`Application` reaches outward only through abstractions it owns, such as
`ICurrentUser`, which outer layers implement.

`tests/SocialMedia.ArchitectureTests/DependencyTests.cs` enforces the project
reference rules and keeps EF Core and Npgsql out of `Domain` and `Contract`.
These tests run in CI with the rest of the suite.

## Consequences

Easier:

- Business logic can be tested without a database or an HTTP host.
- A dependency rule violation fails the tests instead of relying on review.
- Every feature has the same shape, so it is clear where new code goes.
- Infrastructure can be swapped behind the abstractions `Application` owns.

Harder:

- Each feature needs more files: a request, handler, validator, DTO and
  tests, even for a trivial endpoint.
- Entities have to be mapped to DTOs.
- MediatR adds indirection, so the call flow from controller to handler is
  less direct to follow.
- We depend on the MediatR library, currently pinned to 12.5.0. Later
  versions have different licensing terms, which we must review before
  upgrading.
