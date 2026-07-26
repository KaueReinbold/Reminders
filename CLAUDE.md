# CLAUDE.md

Guidance for Claude Code when working in this repository.

> Architecture, service layout, and coding standards live in [agents.md](agents.md). Read it before making structural changes. This file covers **workflow rules only**.

## Project Context

Professional portfolio project. Purpose: demonstrate system design skills and technologies being learned. Code quality and clear history matter more than feature velocity: every change should be something worth showing.

## Backlog

GitHub Issues is the backlog. Work items, bugs, and ideas are tracked as issues in `jumperck/Reminders`. Reference issues in PRs (`Closes #123`). Check open issues before proposing new work.

**Backlog first**: at session start, run `gh issue list` to see current backlog. Every piece of work should map to an issue; if none exists, propose creating one before coding.

### Labels

Five dimensions. Every issue gets exactly one type label; add scope/initiative/status/phase as applicable. Do not create new labels without updating this table.

| Dimension | Labels | Rule |
|---|---|---|
| Type | `feature`, `task`, `bug`, `chore`, `docs`, `dependencies` | Exactly one. `feature` = parent plan or standalone idea; `task` = child of a feature |
| Scope | `api`, `go`, `react`, `mvc`, `blockchain`, `infra`, `ci`, `flutter`, `testing` | Mirrors commit scopes; add `cpp`/`migrations` when first needed |
| Initiative | `audit`, `redesign`, `learning` | Groups related work |
| Status | `triage`, `hold`, `duplicate`, `invalid` | Workflow state |
| Phase | `phase-1`, `phase-2`, `phase-3` | Audit plan only |

### Project board

[Project 7](https://github.com/users/jumperck/projects/7) is the execution view of the backlog (see ADR-0002). Columns: Backlog, Todo, In Progress, Done. Extra fields: Priority (P0/P1/P2), Initiative (audit/redesign/learning).

- When claiming an issue, move its board item to In Progress.
- New issues are auto-added to the board; set Priority and Initiative when triaging.
- Closed issues move to Done automatically; do not set Done by hand.
- Todo means prioritized and ready to start; Backlog means not yet prioritized.

### Sprints and check-in ritual

Work is scheduled on the board's `Sprint` iteration field (2-week cadence). Agents act as scrum master at session start:

1. Determine the current sprint from today's date (`gh project field-list 7 --owner jumperck` for iterations).
2. Report sprint progress: items Done / In Progress / not started in the current sprint.
3. Flag spillover: unfinished items from past sprints and unscheduled Todo items.
4. Propose carryover or rescheduling; the user decides. Never move sprint dates or reassign items without explicit approval.

Rules:
- Assign prioritized (Todo) issues to a sprint; Backlog/P2 items stay unscheduled; parent plan issues never get a sprint.
- Sprint dates and durations are set by the maintainer; do not change them.
- Warning: rewriting the Sprint field configuration recreates iterations and wipes all item assignments; reassign afterwards.

Sprint naming: each quarter has a theme chosen by the maintainer; sprint names take sequential alphabetical initials, and the sequence continues across quarters (Q3 ends at E, Q4 starts at F). 2026 Q3/Q4 theme: Lord of the Rings (Aragorn ... Isildur). When creating sprints for a new quarter, ask the maintainer for the theme and continue from the last letter used.

## Architecture Decision Records (ADRs)

Decisions persist in `docs/adr/` so all agents share the same memory. Format: `NNNN-short-title.md` (see `docs/adr/0000-template.md`).

- Write an ADR for any decision that shapes architecture, workflow, or tooling (new dependency, new service, pattern change, process rule).
- Read existing ADRs before proposing changes that might contradict them. Superseding an old ADR: new ADR references it, old one gets status `superseded`.
- ADRs are short: context, decision, consequences. One page max.

## Multi-Agent Coordination

Multiple agents may work this repo in parallel. Rules:

- Claim work by assigning yourself or commenting on the issue before starting.
- One issue = one branch = one PR. Never share branches between agents.
- Do not start an issue already claimed by another agent (check assignees/comments).
- State that outlives your session goes in: GitHub Issues (status, findings), ADRs (decisions), PR descriptions (implementation notes). Never assume other agents share your conversation context.

## Development Workflow: Trunk-Based Development

- `main` is the trunk. It must always be releasable (green CI, deployable).
- Work in short-lived branches off `main`: merge within 1-2 days max. No long-lived feature branches.
- Branch naming: `<type>/<short-description>` (e.g. `feat/redis-cache`, `fix/async-service-chain`, `ci/infra-validation`).
- Keep PRs small and focused: one logical change per PR.
- Rebase on `main` before merging; prefer squash merge to keep trunk history linear.
- Never commit directly to `main`: always via PR so CI workflows validate the change.
- Incomplete features: hide behind configuration/profile, never leave trunk broken.

## Conventional Commits

All commits and PR titles follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <description>
```

**Types**: `feat`, `fix`, `refactor`, `test`, `docs`, `ci`, `chore`, `perf`, `build`

**Scopes** (match service/area):
- `api`: .NET API (`src/server/api/dotnet/`)
- `go`: Go API
- `cpp`: C++ API
- `react`: Next.js app
- `mvc`: ASP.NET MVC app
- `blockchain`: Solidity/Hardhat
- `migrations`: MigrationsRunner / EF migrations
- `infra`: Docker, Nginx, k6
- `ci`: GitHub Actions

Examples:
- `feat(api): add Redis caching for reminder queries`
- `fix(api): remove blocking .Result calls in RemindersService`
- `ci(infra): validate docker compose config on PR`

Rules:
- Subject: imperative mood, lowercase, no trailing period, ≤72 chars.
- Body explains **why** when not obvious from the diff.
- Breaking changes: `!` after type/scope and a `BREAKING CHANGE:` footer.

## Versioning & Tags

Semantic Versioning (`vMAJOR.MINOR.PATCH`). Existing tags: `v1.0.0` … `v5.0.0`.

**Git tags**: created on `main` after merging a release-worthy change:
- `MAJOR`: breaking API/schema changes
- `MINOR`: new features (`feat`)
- `PATCH`: fixes (`fix`), small improvements

```bash
git tag -a v5.1.0 -m "feat(api): describe the release"
git push origin v5.1.0
```

**Docker image tags**: mirror the git tag plus a moving `latest`:

```bash
docker build -t reminders-api:v5.1.0 -t reminders-api:latest src/server/api/dotnet/
```

Rules:
- Never retag/move a published version tag: publish a new one.
- Docker version tags are immutable; only `latest` moves.
- Ask before creating/pushing tags: releases are a user decision.

## Commands

```bash
# Full stack
docker compose --profile all up --build -d

# Backend only
docker compose --profile api up -d

# .NET tests
dotnet test src/test/server/dotnet/Reminders.Application.Test/

# React tests
cd src/app/reactjs/reminders-app && npm test

# Blockchain tests
cd blockchain && npx hardhat test

# Validate compose files
docker compose config -q
```

## Writing Style

Never use em dash (—) or en dash (–) anywhere: docs, comments, commit messages, issues, PRs, AI responses. Use colon, comma, or hyphen instead.

## Rules for Claude

- **Always on, every session in this repo**: caveman (terse output: drop filler, keep all technical substance) and ponytail (minimal code: smallest change that works, prefer platform/stdlib over new dependencies, delete before adding).
- Read `agents.md` for architecture patterns before editing service code.
- Database changes require migrations for **both** Postgres and SqlServer providers (see agents.md).
- Do not commit, push, tag, or open PRs unless explicitly asked.
- Run the relevant test suite after code changes.
- No secrets in code or compose files: use `.env` (gitignored); `.env.example` holds placeholders only.
