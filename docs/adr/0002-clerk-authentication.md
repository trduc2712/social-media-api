# 2. Use Clerk for Authentication

Date: 2026-10-08

## Status

Accepted

## Context

The API needs to know which user is making each request. Building sign-up,
sign-in, password storage, email verification, social login and session
handling ourselves is a large amount of security-sensitive work that is not
specific to a social media app.

## Decision

We delegate identity to Clerk. The web app signs users in with Clerk and
sends the session token Clerk issues as a bearer token on every API request.

The API does not issue tokens or store credentials. It only validates
Clerk's JWTs with the ASP.NET Core JwtBearer handler:

- The signing keys and issuer come from the Clerk instance configured in
  `Clerk:Authority`. The API refuses to start without it.
- Audience validation is off, because Clerk session tokens carry no audience
  for this API. Instead, when a token has an `azp` claim, it must be one of
  `Clerk:AuthorizedParties`.
- A fallback authorization policy requires an authenticated user on every
  endpoint unless the endpoint opts out.

The Clerk user ID, taken from the `sub` claim, is the identity of a user in
this system. `Application` reads it only through `ICurrentUser`, which `Api`
implements, so no layer below `Api` knows about Clerk or JWTs.

Swagger UI loads Clerk's script when `Clerk:PublishableKey` is set, so a
developer can sign in there and call the API with a real token.

## Consequences

Easier:

- We do not build, secure or maintain sign-up, sign-in, password reset or
  social login.
- The API stays stateless: it holds no sessions and no secrets for token
  validation, only public keys fetched from Clerk.
- Tests do not need Clerk. The functional tests replace the signing keys and
  issue their own tokens.

Harder:

- We depend on Clerk for availability and pricing. If Clerk is down, nobody
  can sign in.
- Moving away later is expensive, because the data we store about users will
  be keyed by the Clerk user ID.
- Profile data such as email and name lives in Clerk. To use it in the API we
  must read it from token claims or copy it into our database and keep it in
  sync.
- The API cannot end a session itself. A token stays valid until it expires,
  so we rely on Clerk's short token lifetime.
