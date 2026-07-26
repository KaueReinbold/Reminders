# Handoff: Reminders App Redesign

## Overview
A fresh, full redesign of the Reminders CRUD frontend (currently a Material UI table app in `jumperck/Reminders`, `src/app/reactjs/reminders-app`). It replaces the plain table with a warm, editorial productivity UI: a grouped reminder list (Overdue / Today / Upcoming / Done), search, view filters, week-progress meter, create/edit modal, delete confirmation, empty states, and a dedicated mobile layout.

## About the Design Files
The files in this bundle are **design references created in HTML** — prototypes showing intended look and behavior, not production code to copy directly. The task is to **recreate these designs in the existing Next.js/React app** (`reminders-app`), using its established patterns (App Router pages, the existing `api/reminders` service layer and `Reminder` type). Drop the MUI table/components in favor of this design; either plain CSS modules/styled JSX or keep MUI only as unstyled primitives.

## Fidelity
**High-fidelity.** Colors, typography, spacing, and interactions are final. Recreate pixel-perfectly.

## Data model (unchanged)
`Reminder { id: number; title: string; description: string; limitDate: string (YYYY-MM-DD); isDone: boolean }`

## Screens / Views

### 1. Reminders list (desktop ≥760px)
- **Layout**: sticky top header + two-column body, max-width 1280px centered, gutters `clamp(20px, 4vw, 48px)`, column gap `clamp(24px, 4vw, 56px)`.
- **Header** (sticky, canvas bg `#F3F0E9`, bottom border `#E0DACD`): page title "Reminders" (Instrument Serif 30px) + today's date in italic serif 19px `#8C8577`; search pill (surface `#FBFAF7`, border `#E0DACD`, radius 999px, magnifier icon, no focus outline); primary button "New reminder" (ink `#1B1A17` bg, `#F7F5F0` text, pill, hover → `#B4451F`).
- **Sidebar** (210–260px, sticky): nav items All / Today / Upcoming / Done with counts — active: `#E7E2D6` bg, radius 10, 5px accent dot; inactive: transparent, hover `#EBE7DC`. Below a divider: "THIS WEEK" label (11.5px uppercase `#8C8577`), % stat (Instrument Serif 40px), 5px progress bar (track `#E0DACD`, fill `#B4451F`), caption 13.5px `#6E6759` (shows overdue count if any).
- **List**: sections per group (Overdue, Today, Upcoming, Done — ordered, empty groups hidden). Section header: serif 22px title + count + 1px rule filling the row. Cards gap 10.
- **Reminder card**: surface `#FBFAF7`, border `#E7E2D6` (hover `#CFC7B5`), radius 14, padding 16px 18px, `display:flex; align-items:center; gap:14px`.
  - Checkbox: 24px circle. Open: `1.6px solid #C6BDA9`, hover green tint `#EDF1EC`. Done: solid `#4C6B4F` with white check.
  - Title 15.5px/500 (done: `#9A9385` + line-through); description 13.8px `#6E6759` below when present.
  - Date pill: overdue = `#B4451F` on `#F6E4DC` ("3 days late" / "Yesterday"); otherwise `#6E6759` on `#F0EDE4` ("Today", "Tomorrow", "Aug 4").
  - Edit: 34px icon button, hover `#EBE7DC`.
- **Empty state**: dashed border `#D5CEBE` panel, radius 18, serif 27px title + 15px body + primary button; copy varies per view/search (see prototype logic).

### 2. Reminders list (mobile <760px)
- Sticky header: title row (serif 25px + short date), full-width search pill, horizontally scrolling filter chips (active = ink pill w/ light text; inactive = surface w/ border). 40px min height chips.
- Progress compresses to a strip under the header: panel bg `#EFEBE1`, serif 22px %, inline bar, caption right-aligned.
- Same cards, single column.
- FAB "New" bottom-right: ink pill, 52px min height, shadow `0 10px 26px rgba(27,26,23,0.28)`.
- In the prototype, mobile is showcased inside an iPhone frame (`ios-frame.jsx`) — the frame is presentation only, not part of the app.

### 3. Create / Edit modal
- Centered overlay, scrim `rgba(27,26,23,0.34)`, click-outside closes; card max-width 560px, `#FBFAF7`, radius 20, padding 28px, entrance `sheetIn` 0.22s translateY(16px)+fade.
- Title "New reminder" / "Edit reminder" (serif 27px) + "Close" text button.
- Fields (gap 18): Title (text), Description (textarea, 3 rows), then Limit date (native date input, pointer cursor incl. calendar icon) + Status toggle side by side (flex, wrap). All inputs: `1px solid #DDD7C9`, bg `#F7F5F0`, radius 10, **fixed height 48px** (textarea excepted), focus = 2px `#B4451F` outline. Labels 11.5px uppercase `#8C8577`.
- Status toggle: button with 20px rounded-square check (green ✓ when done) + "Done" / "Not done yet".
- Footer: primary "Create reminder"/"Save changes", secondary "Cancel" (border pill), and on edit a right-aligned text-danger "Delete reminder".
- Save disabled/no-op when title is blank.

### 4. Delete confirmation
- Smaller centered dialog, max-width 400px, radius 16, scrim `rgba(27,26,23,0.42)`.
- Serif 24px "Delete this reminder?", body quotes the reminder title, buttons: destructive "Delete" (`#B4451F` bg, hover `#8E3415`) + secondary "Keep it".

## Interactions & Behavior
- Checkbox toggles `isDone` instantly (optimistic update; card moves group).
- Card edit icon → edit modal prefilled; "New reminder" / FAB → create modal with limitDate defaulting to tomorrow.
- Search filters title + description live; view filters: All / Today (due today) / Upcoming (future, not done) / Done.
- Items sorted ascending by limitDate within groups.
- Overdue = not done && limitDate < today.
- Hover states on every clickable; pointer cursor everywhere, incl. date input + its calendar icon.
- Modals: fadeIn 0.16s scrim, sheetIn 0.22s cubic-bezier(.22,.9,.25,1) card; Esc/scrim-click close is expected in production.
- Responsive: breakpoint 760px switches shells (sidebar ↔ chips+FAB). In production use a CSS media query or container query rather than the prototype's JS resize listener.

## State Management
- `items: Reminder[]` (from existing API), `query`, `view` ('All'|'Today'|'Upcoming'|'Done'), `draft` (modal form), `sheet` ('create'|'edit'|null), `confirmId`.
- Derived: grouped/filtered lists, week progress (% done of items due within 7 days), overdue count.
- Wire create/update/delete/toggle to the existing REST endpoints; prototype uses local state only.

## Design Tokens
See `design-system.md` (bundled) — canonical palette, type scale, radii, spacing, and component recipes. Fonts: Google Fonts **Instrument Serif** (display) + **DM Sans** (UI).

## Assets
No image assets. Icons are tiny inline SVGs (plus, magnifier, pencil, check) — copy paths from the prototypes or use an icon library equivalent. iPhone bezel (`ios-frame.jsx`) is presentation-only.

## Files
- `Reminders.dc.html` — the full prototype (single source: template markup + logic class). All exact styles are inline here.
- `Reminders Desktop.dc.html`, `Reminders Tablet.dc.html`, `Reminders Mobile.dc.html` — thin wrappers forcing a layout (mobile adds the iPhone frame).
- `ios-frame.jsx` — device frame used by the mobile showcase.
- `design-system.md` — design tokens & component recipes.
