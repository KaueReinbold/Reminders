# ADR-0009: Flutter API client on the http package

- **Status**: accepted
- **Date**: 2026-08-22
- **Issue**: #337 (parent #325)

## Context

The Flutter app needs a REST client for `/api/reminders` behind nginx. It must be unit-testable without a server and keep the dependency footprint small (ponytail).

## Options considered

### Option 1: `package:http`

Dart team package, thin wrapper over `dart:io`/browser clients, `MockClient` in `http/testing.dart` for tests. Costs: one dependency. Buys: injectable client, trivial mocking, works on every Flutter platform.

### Option 2: `dart:io` `HttpClient` directly

No dependency, but tests need a real local server or a hand-written fake, and the code does not run on web.

### Option 3: `dio`

Interceptors, retries, cancellation. More than the app needs today; heavier dependency.

## Decision

Option 1. `RemindersApi` takes an `http.Client` (default `http.Client()`), tests inject `MockClient`. Errors map to `ApiException` with status, message and per-field validation errors. Base URL comes from `--dart-define=API_BASE_URL`.

## Consequences

- Easier: fast unit tests for every call and error path; swapping the client for retries or logging later is one constructor argument.
- Harder: nothing notable; `http` is maintained by the Dart team.
- Watch: keep the client thin; add `dio` only if interceptors or cancellation become a real need.
