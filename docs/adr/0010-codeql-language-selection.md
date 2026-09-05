# ADR-0010: CodeQL analyzes only the languages a pull request changed

- **Status**: accepted
- **Date**: 2026-09-05
- **Issue**: #407 (found while fixing the Flutter CI pin)

## Context

CodeQL runs in advanced setup with a three-language matrix: `actions`, `csharp`,
`javascript-typescript`. The workflow triggers on any pull request that is not
docs-only, and the matrix has no per-language filter, so every leg runs on every
change. A pull request touching one workflow file spent about two minutes
scanning C# nobody had edited, and about one minute on JavaScript, on every push
to the branch.

Constraints:

- Advanced setup means the matrix lives in the workflow, so filtering is ours to
  write.
- CodeQL analyzes the whole checkout, not the diff. Skipping a language on a
  pull request means that language has no fresh result for that branch, not that
  a result is lost: the baseline comes from `main`.
- Only `Validate conventional commit title` is a required status check on `main`,
  so a matrix leg that never starts cannot leave a check stuck as expected.

## Options considered

### Option 1: Leave it

Zero work, complete coverage on every branch. Costs about three runner-minutes
per unrelated pull request and pushes the review agent's wait gate out by the
slowest leg.

### Option 2: `dorny/paths-filter` gate job

Well-known action, declarative filters. Adds a third-party action to the
supply chain for a job that a dozen lines of shell already do.

### Option 3: Resolve the matrix from the changed files (chosen)

A `languages` job lists the pull request's files through `gh api` and emits the
matrix as JSON. `push` to `main` and the weekly schedule keep all three
languages.

## Decision

Option 3. The `languages` job maps changed files to languages
(`.github/workflows|actions/` to `actions`; `.cs .csproj .sln .props .targets` to
`csharp`; `.js .jsx .ts .tsx .mjs .cjs .json` to `javascript-typescript`) and the
`analyze` job consumes that list. No new dependency: `gh` is preinstalled on the
runner.

Every language keeps a full baseline because pushes to `main` and the Monday
schedule always analyze all three.

## Consequences

- Unrelated pull requests get their CodeQL time back, and the review agent's
  wait gate clears sooner.
- A pull request touching none of the three languages (Dart, Solidity, C++,
  Docker) skips `analyze` entirely. Safe only while no per-language check is
  required on `main`: making `Analyze (csharp)` required would leave such a pull
  request waiting forever. Required checks and this workflow must be changed
  together.
- The extension map is now a thing to maintain. A new language in the matrix, or
  a source extension nobody listed, needs an edit here or its analysis silently
  stops running on pull requests.
