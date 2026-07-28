# Product Docs

Product intent and outcomes are recorded here so features trace from idea to validated result. Direction lives in [vision.md](vision.md). Each feature-sized effort gets a one-page PRD following [0000-template.md](0000-template.md): problem, users, success criteria, scope, non-goals, validation.

## Index

| PRD | Title | Status | Date |
|---|---|---|---|

## Process

1. Direction starts as an Ideas discussion.
2. Draft the PRD as `NNNN-short-title.md` with status `draft`, in a PR.
3. Merge when scope is agreed: status becomes `active`; open the parent feature issue linking the PRD.
4. Technical decisions made during implementation land as ADRs referencing the PRD.
5. At release: status becomes `shipped`. Fill the Validation section with evidence: status becomes `validated`.
6. Add every new PRD to the index table above in the same PR.

Small fixes and chores do not need a PRD.
