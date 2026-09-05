# Architecture Decision Records

Decisions that shape architecture, workflow, or tooling are recorded here so humans and agents share the same memory. Each ADR follows [0000-template.md](0000-template.md): context, options considered with trade-offs, decision, consequences. One page max.

## Index

| ADR | Title | Status | Date |
|---|---|---|---|
| [0001](0001-development-workflow.md) | Development workflow: trunk-based, conventional commits, issues as backlog | accepted | 2026-07-26 |
| [0002](0002-project-board.md) | GitHub Project board as execution view of the backlog | accepted | 2026-07-26 |
| [0003](0003-multi-implementation-api.md) | Same REST API in .NET, Go and C++ behind Nginx | accepted | 2026-07-28 |
| [0004](0004-dual-database-migrations-runner.md) | Dual database providers with a dedicated migrations runner | accepted | 2026-07-28 |
| [0005](0005-blockchain-integration.md) | Solidity contracts with a local Ganache node in the stack | accepted | 2026-07-28 |
| [0006](0006-product-docs-and-prd-workflow.md) | Product docs and PRD workflow | accepted | 2026-07-28 |
| [0007](0007-agent-contribution-workflow.md) | Agent contribution workflow: worktrees, quality gates, e2e upkeep | accepted | 2026-08-18 |
| [0008](0008-pr-review-agent.md) | PR review agent via Claude Code Action | accepted | 2026-08-22 |
| [0009](0009-flutter-http-client.md) | Flutter API client on the http package | accepted | 2026-08-22 |
| [0010](0010-codeql-language-selection.md) | CodeQL analyzes only the languages a pull request changed | accepted | 2026-09-05 |
| [0011](0011-api-validation-contract.md) | One validation and error contract across the .NET, Go and C++ APIs | accepted | 2026-09-05 |
| [0012](0012-react-mock-api-for-pages-demo.md) | In-browser mock API for the GitHub Pages demo | accepted | 2026-09-05 |
| [0013](0013-in-place-reminder-overlays.md) | In-place overlays for reminder create, edit and delete | accepted | 2026-09-05 |

## Process

1. Ideas that need debate start as a GitHub Discussion (RFC).
2. Draft the ADR as `NNNN-short-title.md` with status `proposed`, in a PR.
3. Review and merge: status becomes `accepted`.
4. Superseding: the new ADR references the old one; the old one gets status `superseded by ADR-NNNN`.
5. Add every new ADR to the index table above in the same PR.
