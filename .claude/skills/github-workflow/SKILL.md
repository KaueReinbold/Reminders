---
name: github-workflow
description: Manage the kauereinbold/Reminders backlog on GitHub: issues, labels, Project 7 board (status, priority, sprints), discussions, and PRs. Use when claiming work, triaging, running the sprint check-in ritual, or opening/merging PRs.
---

# GitHub workflow for kauereinbold/Reminders

Repo: `kauereinbold/Reminders`. Backlog lives in GitHub Issues, execution view is Project 7. Views: [board](https://github.com/users/kauereinbold/projects/7) (columns) and [roadmap](https://github.com/users/kauereinbold/projects/7/views/2) (timeline by sprint; link this when pointing people at the plan). Rules below complement CLAUDE.md (label taxonomy, sprint ritual, commit conventions); this skill holds the command recipes.

## Environment notes

- `jq` is NOT installed. Use `gh --jq` (built-in) instead of piping to jq.
- Never add AI attribution to commits, PRs, or comments.
- No em/en dashes anywhere.

## Stable IDs (Project 7)

Fetch fresh if commands fail; these are stable otherwise.

| Thing | ID |
|---|---|
| Project | `gh project view 7 --owner kauereinbold --format json --jq .id` |
| Status field | `PVTSSF_lAHOAVmfxM4AaN1gzgQza3U` |
| Status: Backlog | `d614a99e` |
| Status: Todo | `91176ea2` |
| Status: In Progress | `05b1b492` |
| Status: Done | `4a5bc286` |
| Sprint field | `PVTIF_lAHOAVmfxM4AaN1gzhY6R9M` |

## Issues

```bash
gh issue list --limit 50                      # backlog scan (session start)
gh issue view <n>                             # details
gh issue edit <n> --add-assignee @me          # claim (required before starting)
gh issue create --title "type(scope): desc" --label <type>,<scope> --body "..."
```

Claim ritual: assign yourself + move board item to In Progress. No claiming comment. One issue = one branch = one PR. Never start an issue already assigned or claimed in comments.

## Project board

List items with fields:

```bash
gh project item-list 7 --owner kauereinbold --format json --limit 100 \
  --jq '.items[] | "\(.content.number)\t\(.status)\t\(.priority // "-")\t\(.sprint.title // "-")\t\(.title)"'
```

Filter one sprint: add `| select(.sprint.title=="<Name>")` before the string.

Get item id for issue N:

```bash
gh project item-list 7 --owner kauereinbold --format json --limit 100 \
  --jq '.items[] | select(.content.number==N) | .id'
```

Move item status (example: In Progress):

```bash
gh project item-edit --id <ITEM_ID> \
  --project-id "$(gh project view 7 --owner kauereinbold --format json --jq .id)" \
  --field-id PVTSSF_lAHOAVmfxM4AaN1gzgQza3U --single-select-option-id 05b1b492
```

Rules: never set Done by hand (closes auto-move). Set Priority and Initiative when triaging. New issues auto-add to board.

## Sprints

`gh project field-list` does NOT return iterations. Use GraphQL:

```bash
gh api graphql -f query='{ user(login:"kauereinbold") { projectV2(number:7) {
  field(name:"Sprint") { ... on ProjectV2IterationField {
    configuration { iterations { id title startDate duration }
                    completedIterations { id title startDate duration } } } } } } }' \
  --jq '.data.user.projectV2.field.configuration | (.iterations[], .completedIterations[]) | "\(.title): \(.startDate) +\(.duration)d"'
```

Assign item to sprint (iteration id from query above):

```bash
gh project item-edit --id <ITEM_ID> --project-id <PROJECT_ID> \
  --field-id PVTIF_lAHOAVmfxM4AaN1gzhY6R9M --iteration-id <ITERATION_ID>
```

Rules: never move sprint dates or reassign items without user approval. Rewriting Sprint field config wipes all assignments. Parent plan issues get no sprint; Backlog/P2 stay unscheduled. Sprint check-in ritual at session start: report Done / In Progress / not started, flag spillover, propose carryover, user decides.

## PRs

```bash
gh pr create --title "type(scope): desc" --body "..."   # body: Summary, Closes #N, Test plan
gh pr checks <n>                                        # CI status
gh pr merge <n> --squash --delete-branch                # after user approves merge
gh api repos/kauereinbold/Reminders/pulls/<n>/comments      # review comments
```

Rules: branch `<type>/<short-description>` off main, PR title is conventional commit (validated by CI), `Closes #N` in body, squash merge, merge only when user says so. Run relevant tests before pushing.

## Discussions

No dedicated gh subcommand; use GraphQL:

```bash
# list
gh api graphql -f query='{ repository(owner:"kauereinbold", name:"Reminders") {
  discussions(first:20, orderBy:{field:UPDATED_AT, direction:DESC}) {
    nodes { number title category { name } updatedAt } } } }' \
  --jq '.data.repository.discussions.nodes[] | "\(.number)\t\(.category.name)\t\(.title)"'

# read one
gh api graphql -f query='{ repository(owner:"kauereinbold", name:"Reminders") {
  discussion(number: N) { title body comments(first:50) { nodes { author { login } body } } } } }'

# comment (discussion node id from: discussion(number:N) { id })
gh api graphql -f query='mutation { addDiscussionComment(input:{discussionId:"<NODE_ID>", body:"..."}) { comment { url } } }'
```

## Releases and tags

Ask the user before creating or pushing any tag. SemVer, tags on main after merge. Docker version tags immutable, only `latest` moves.
