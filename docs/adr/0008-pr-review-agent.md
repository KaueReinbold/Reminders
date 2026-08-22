# ADR-0008: PR review agent via Claude Code Action

- **Status**: accepted
- **Date**: 2026-08-22
- **Issue**: #351 (parent #356)

## Context

Every PR in this repo is reviewed by the maintainer, often after an agent wrote it. A second, independent pass against the repo rules (CLAUDE.md, agents.md, ADRs) before the human review catches rule drift early: dashes, missing migrations, stale Cypress specs, missing ADRs. #356 frames agents operating inside the SDLC through GitHub automation with one guardrail: agents comment and propose, humans merge.

## Options considered

### Option 1: Claude Code GitHub Action (`anthropics/claude-code-action@v1`)

Official action, runs Claude Code in the workflow with the PR checked out, posts inline and summary comments through the GitHub API. Costs: one repo secret, a workflow to maintain, usage per PR run (billed to the maintainer subscription via an OAuth token from `claude setup-token`, or to the API when an API key is used instead). Buys: full repo context (reads CLAUDE.md and agents.md), tool allow-list, no custom code.

### Option 2: Custom script calling the Messages API with the diff

Smaller dependency surface, but reimplements checkout, diff chunking, comment posting, and has no repo-wide context beyond what the script sends. More code to own for less review quality.

### Option 3: Third-party review bots

Least setup, but opaque prompts, no alignment with repo rules, another vendor account.

## Decision

Option 1. Workflow `.github/workflows/claude-pr-review.yml` runs on `pull_request` (opened, synchronize, reopened, ready_for_review), skips Dependabot, drafts and forks, and is limited to read the repo plus post comments (`gh pr comment`, inline comment tool). The prompt points at CLAUDE.md and agents.md and requires severity-tagged findings ([blocker], [major], [minor], [nit]). The action never approves, requests changes, pushes, or merges. Auth is the maintainer subscription: `CLAUDE_CODE_OAUTH_TOKEN` repo secret generated with `claude setup-token` (input `claude_code_oauth_token`); switching to `ANTHROPIC_API_KEY` is a one-line change.

## Consequences

- Easier: consistent first-pass review against repo rules on every PR; maintainer review starts from tagged findings.
- Harder: subscription usage per PR push (concurrency cancels superseded runs); the OAuth token is tied to one person and expires (about a year), so it must be rotated; prompt drift if CLAUDE.md changes without revisiting the prompt.
- Watch: noisy nits (tighten the prompt, not the severity), fork PRs get no review by design, the action version pin (`@v1`) moving under us.
