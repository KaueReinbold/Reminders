# ADR-0001: Development workflow: trunk-based, conventional commits, issues as backlog

- **Status**: accepted
- **Date**: 2026-07-26

## Context

Portfolio project maintained by one developer plus multiple AI agents working in parallel. Needs a workflow that keeps history clean, keeps `main` releasable, and lets agents coordinate without shared conversation context.

## Decision

- Trunk-based development: short-lived branches off `main`, squash merge via PR, `main` always green.
- Conventional Commits for all commits and PR titles, with service scopes (api, go, cpp, react, mvc, blockchain, migrations, infra, ci).
- GitHub Issues is the single backlog. Every change maps to an issue; PRs close issues.
- SemVer git tags (`vX.Y.Z`) on `main`; Docker image tags mirror git tags, version tags immutable, only `latest` moves.
- Decisions recorded as ADRs in `docs/adr/` so agents and humans share persistent memory.
- Multi-agent rule: claim issue before starting, one issue = one branch = one PR.
- Writing style: no em/en dashes anywhere.
- AI agents run caveman (terse output) and ponytail (minimal code) always on in this repo.

## Consequences

- Easier: parallel agent work without collisions, readable history, reviewable small PRs.
- Harder: every change needs an issue first; small friction for tiny fixes.
- Watch: stale claimed issues; unassign if abandoned.
