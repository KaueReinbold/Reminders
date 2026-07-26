# ADR-0002: GitHub Project board as execution view of the backlog

- **Status**: accepted
- **Date**: 2026-07-26

## Context

GitHub Issues holds the backlog (ADR-0001), but with 40+ open issues across audit, redesign, and learning initiatives, labels alone do not show what is prioritized, in flight, or next. Multiple agents work in parallel and need a shared, current picture of execution state.

## Decision

- GitHub Project 7 ("Reminders") is the single execution board for `kauereinbold/Reminders`.
- Status columns: Backlog (not prioritized), Todo (prioritized, ready), In Progress (claimed), Done (closed).
- Extra single-select fields: Priority (P0/P1/P2) and Initiative (audit/redesign/learning), mirroring initiative labels.
- Built-in workflows keep the board current: issues auto-added on open, moved to Done on close, back to Todo on reopen.
- Agents move their item to In Progress when claiming an issue and set Priority/Initiative when triaging new issues. Done is automated, never set by hand.

## Consequences

- Easier: at-a-glance execution state for humans and agents; prioritization separate from labels.
- Harder: one more surface to keep honest; stale In Progress items need periodic sweep.
- Watch: board and labels drifting apart on initiative; labels stay the source of truth for taxonomy.
