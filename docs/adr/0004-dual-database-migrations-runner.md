# ADR-0004: Dual database providers with a dedicated migrations runner

- **Status**: accepted
- **Date**: 2026-07-28

## Context

The API supports PostgreSQL (default) and SQL Server, each with its own EF Core migration set. Schema changes must be applied exactly once and before any API instance starts, in a stack where three API implementations share one database.

## Options considered

### Option 1: API applies migrations at startup

Simple and common in single-service apps. Breaks down here: multiple API instances race to migrate, and API startup gets coupled to migration success.

### Option 2: Manual migrations

Full control over when schema changes apply, but easy to forget and impossible for agents and CI to reproduce deterministically.

### Option 3: Dedicated migrations runner service

A .NET console app that runs once per deployment. Compose starts `postgres`, waits for it to be healthy, runs the runner to completion, then starts the APIs (`service_completed_successfully`). Retry with exponential backoff; an HTTP health endpoint reports progress. Cost: one more service to maintain.

## Decision

Option 3. Migrations live per provider under `Layers/Data/EntityFramework/{Postgres,SqlServer}/Migrations/`. Every schema change must add migrations for both providers.

## Consequences

- Easier: deterministic startup order; no migration races; APIs stay free of migration logic.
- Harder: one more service; both migration sets must stay in sync.
- Watch: when PostgreSQL is active, the runner logs an expected failure for the SQL Server set; this is harmless by design.
