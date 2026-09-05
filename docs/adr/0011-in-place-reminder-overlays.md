# ADR-0011: In-place overlays for reminder create, edit and delete

- **Status**: accepted
- **Date**: 2026-09-05
- **Issue**: #330 (parent #322)

## Context

The React app created and edited reminders on dedicated routes, `/reminder/create` and `/reminder/edit`, both built from MUI components. The redesign handoff for the reminders list specifies a create/edit sheet and a delete confirmation dialog that open over the list, keeping the list visible behind a scrim.

Two forces meet here. The redesign wants overlays rather than page navigation. The wider redesign initiative is also moving the app off MUI onto the design system tokens already defined in `globals.css`, which is tracked separately as #332.

## Options considered

### Option 1: Keep the routes, render them as MUI `Modal`

Wrap the existing pages in MUI `Modal` and keep routing. Buys the smallest diff and MUI's built-in focus trap. Costs: keeps a dependency the redesign is removing, and MUI's own styling fights the design system tokens, so the sheet would not match the handoff without heavy overrides.

### Option 2: Overlay state on the list page, plain CSS modules

The list page owns the overlay state and calls the mutations directly. The sheet and dialog are plain components styled with CSS modules over the design tokens. Buys an exact match to the handoff and no new dependency. Costs: focus management that MUI `Modal` used to provide has to be written by hand.

## Decision

Option 2. `reminder/list/page.tsx` holds the sheet and confirmation state and calls the create, update and delete mutations, invalidating the reminders query on success. `ReminderSheet` and `ReminderDeleteModal` are plain CSS module components on the design tokens, with no MUI.

Focus and keyboard behaviour is covered by two small hooks rather than a dependency: `useEscapeKey` closes on Escape, `useReturnFocus` returns focus to the trigger on close, and each overlay autofocuses its first control.

The legacy routes, `ReminderForm`, `AlertError` and the reminder context stay in place for now, still covered by their specs. Removing them together with MUI is #332, so trunk keeps working through both changes.

## Consequences

Create, edit and delete no longer navigate, so the list keeps its scroll position, search text and view filter across an edit. API errors keep the overlay open instead of stranding the user on a separate route.

Two rendering paths for the same form exist until #332 lands, and both need their specs kept green. Overlay accessibility is now the repo's responsibility: the current hooks give Escape, autofocus and focus restore, but not a full tab trap, which is worth revisiting once the legacy routes are gone.
