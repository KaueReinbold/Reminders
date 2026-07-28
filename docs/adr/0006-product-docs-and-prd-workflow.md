# ADR-0006: Product docs and PRD workflow

- **Status**: accepted
- **Date**: 2026-07-28
- **Issue**: #367

## Context

The workflow records technical decisions (ADRs) and execution state (issues, project board), but nothing records product intent or outcomes. Ideas discussions (#363, #364, #365, #366) hold feature-sized directions with no defined path from idea to shipped, validated work. Feature issues state acceptance criteria but not the user problem or how success is measured after release.

## Options considered

### Option 1: Discussions and issues only

Keep product intent in discussions and issue bodies. Buys zero new artifacts; costs intent scattered across threads, no record of whether shipped features achieved anything.

### Option 2: Full product documentation

Roadmap, personas, OKRs, specs. Buys thoroughness; costs heavy upkeep for a solo project and duplicates what the board already does.

### Option 3: Lean PRDs mirroring the ADR pattern

One-page PRDs in `docs/product/` with template, index, and lifecycle, plus a one-page vision. Buys traceability from idea to validated outcome at minimal cost.

## Decision

Option 3. The flow for feature-sized work: Ideas discussion, then a PRD (problem, users, success criteria, scope, non-goals), then ADRs for technical decisions made while building, then issues, PRs, and release, with validation evidence recorded back in the PRD. PRD lifecycle: draft, active, shipped, validated. Small fixes and chores skip the PRD.

## Consequences

Features gain one extra artifact, so starting them costs slightly more. Product intent and outcomes become reviewable in the repo instead of living in threads. The Validation section forces a post-release check against the stated success criteria. Vision stays a compass: the board and discussions remain the roadmap.
