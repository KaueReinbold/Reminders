# ADR-0007: Agent contribution workflow: worktrees, quality gates, e2e upkeep

- **Status**: accepted
- **Date**: 2026-08-18

## Context

Maintainer and multiple AI agents share one local clone. Branch switching in the main checkout risks clobbering uncommitted work (hit while preparing PR #389: the checkout was dirty on `feat/react-grouped-list`, work had to move to a worktree). Cypress specs drift when UI changes merge without spec updates. ADR-0001 defines the macro workflow (trunk, issues, conventional commits); this ADR defines the per-change implementation loop.

## Decision

Every change follows this loop:

1. **Claim**: pick or create the issue, assign yourself, move the board item to In Progress.
2. **Worktree, always**: never branch or checkout in the main working copy. Create an isolated worktree off the trunk:

   ```bash
   git fetch origin
   git worktree add ../wt-<short-description> -b <type>/<short-description> origin/main
   ```

3. **Implement**: follow `agents.md` standards and the design system; minimal change (ponytail); migrations for both Postgres and SqlServer when schema changes.
4. **Quality gates before PR**:
   - Relevant unit suite green (.NET, React/Jest, Flutter, Hardhat per scope).
   - Lint clean (ESLint, `flutter analyze`, actionlint via CI).
   - Infra/compose changes: runtime smoke test (boot profile, hit endpoint, tear down).
   - **Cypress**: any change touching the React UI or the API contract updates the matching specs in `src/test/cypress/cypress/e2e/` and runs them locally against a dev build before the PR (`npm run dev` in the app, then `cd src/test/cypress && npm run cy:run`, `-@api` tags excluded when no backend). A new user-facing flow requires a new spec.
5. **PR**: small, conventional title, `Closes #N`, wait for review; squash merge after approval.
6. **Cleanup**: `git worktree remove` after merge; prune stale worktrees at session start (`git worktree prune`).

## Consequences

- Easier: parallel agent work without clobbering uncommitted state; e2e coverage tracks UI reality.
- Harder: cypress runs add minutes to UI PRs; worktree paths add one indirection.
- Watch: stale worktrees accumulating; specs skipped via tags becoming dead coverage.
