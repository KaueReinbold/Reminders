---
name: ponytail
description: >
  Minimal-code discipline for this repo: the smallest change that works, platform and
  stdlib before new dependencies, delete before adding. Use when writing or reviewing
  code changes, or when someone says "ponytail", "keep it minimal", "smallest change".
---

Smallest change that works. Tight, like a ponytail: nothing loose.

## Rules

- Solve the stated problem only. No speculative abstractions, options, or "while I am here" refactors.
- Prefer platform and stdlib over a new dependency. A new dependency needs a reason the standard library cannot cover, stated in the PR.
- Delete before adding: remove dead code, unused files, stale config in the same change when they touch the edited area.
- Reuse existing patterns in the codebase before inventing a new one. Copy the shape of the nearest similar code.
- One logical change per PR. Unrelated fixes found on the way become issues, or a clearly separated commit when tiny and blocking.
- No new layers, wrappers, or config knobs unless two concrete call sites need them today.
- Tests cover the behaviour added, not the implementation details.

## When reviewing

Flag as a finding:
- Code that is not needed for the issue being closed.
- A new dependency where stdlib or an existing one works.
- Added abstraction with a single caller.
- Left-behind dead code, commented-out code, TODOs without an issue.
- A PR that mixes unrelated changes.

Suggest the smaller version, not a bigger rewrite.

## Boundaries

Minimal is not sloppy: error handling, validation, and tests that the change needs stay in. Readability beats cleverness; a few more plain lines are fine.
