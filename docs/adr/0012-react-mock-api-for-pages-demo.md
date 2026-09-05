# ADR-0012: In-browser mock API for the GitHub Pages demo

- **Status**: accepted
- **Date**: 2026-09-05
- **Issue**: #176

## Context

The React app is exported statically to GitHub Pages (`deploy-pages.yml`). Pages
serves files only: there is no backend behind the demo, so every reminders
request fails and the deployed app is unusable as a portfolio artifact.

The demo must show the full CRUD flow without a server, while the real builds
(Docker Compose, any hosted deployment) keep talking to the live API exactly as
before.

## Options considered

### Option 1: Public API deployment

Host the .NET API somewhere public and point the Pages build at it. Trade-offs:
a genuinely live demo, at the cost of hosting, a database, CORS setup and an
open write endpoint on the internet.

### Option 2: Mocking library (MSW or similar)

Intercept `fetch` with Mock Service Worker. Trade-offs: familiar tooling and
request-level fidelity, at the cost of a new dependency, a service worker to
register under a `basePath`, and machinery the app does not otherwise need.

### Option 3: Mock module behind a build flag

A small in-memory module implementing the same CRUD signatures as the API
client, selected at build time by an environment flag. Trade-offs: no new
dependency and no runtime interception, but the mock has to mirror the client
contract and the server validation by hand.

## Decision

Option 3. `src/app/api/mock.ts` holds seeded reminders and the CRUD functions;
`src/app/api/index.ts` picks the mock or the live `fetch` implementation from
`IS_MOCK_API` (`process.env.NEXT_PUBLIC_MOCK_API === 'true'`). `next.config.js`
turns the flag on for the Pages build (`GITHUB_PAGES=true`) and leaves it off
everywhere else. The header shows a "Demo data" badge whenever the flag is on.

## Consequences

- The Pages demo is fully usable with no backend and no hosting cost.
- Live builds are untouched: the flag resolves to `false` and the API client
  behaves exactly as before.
- The mock reuses `ValidationService`, so demo validation errors match the
  server messages; API contract changes must be mirrored in the mock.
- Mock state is in memory only: a page reload restores the seed data.
- The mock module still ships in the live bundle as unreachable code, a few
  hundred bytes; not worth a bundler `sideEffects` change to strip.
