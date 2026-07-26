# Reminders — Design System (binding for all designs in this project)

All styling is inline (DC rules). These are the ONLY approved values — reuse them verbatim; don't invent new colors, fonts, radii, or sizes.

## Color
- Canvas: `#F3F0E9` (page bg) · Panel strip: `#EFEBE1`
- Surface: `#FBFAF7` (cards, modals, inputs on canvas) · Input fill: `#F7F5F0`
- Ink: `#1B1A17` (primary text, primary buttons) · Body-muted: `#6E6759` · Faint: `#8C8577` · Disabled/done text: `#9A9385`
- Borders: `#E0DACD` (page dividers) · `#E7E2D6` (card) · `#DDD7C9` (inputs) · hover border: `#CFC7B5` / `#C6BDA9`
- Accent (urgency/destructive/links/focus): `#B4451F`, hover `#8E3415`, tint bg `#F6E4DC`
- Success/done: `#4C6B4F`, tint bg `#EDF1EC`
- Overlay scrim: `rgba(27,26,23,0.34–0.42)`

## Type
- Display: `'Instrument Serif', serif` — page title 30px, section headers 22px (weight 400), modal titles 24–27px, stats 40px; italic for dates/asides
- UI/body: `'DM Sans', system-ui, sans-serif` — body 15–15.5px, secondary 13.5–14.5px, pills/badges 12.5px
- Labels: 11.5px, uppercase, letter-spacing 0.09–0.1em, color `#8C8577`
- Google Fonts link required in every DC helmet (Instrument Serif + DM Sans)

## Shape & space
- Radii: pills/buttons `999px` · cards `14px` · modals `16–20px` · inputs `10px` · nav items `10px`
- Card padding `16px 18px`; modal padding `28px`; page gutters `clamp(20px, 4vw, 48px)`
- Sibling groups always flex/grid with `gap` (cards gap 10, sections gap 30, form fields gap 18)

## Components
- Primary button: ink bg, `#F7F5F0` text, pill radius, `padding 12–13px 20–26px`, hover → accent bg
- Secondary button: transparent, `1px solid #DDD7C9`, pill, hover → ink border
- Destructive: accent bg, hover `#8E3415`; text-danger variant: transparent, accent text
- Inputs: `1px solid #DDD7C9`, bg `#F7F5F0`, radius 10, fixed `height: 48px` (textarea excepted); focus = 2px accent outline (suppressed inside search pill via `style-focus="outline: none"`)
- Card row: surface bg, card border, radius 14, `align-items: center`
- Status pills: overdue = accent text on `#F6E4DC`; neutral = `#6E6759` on `#F0EDE4`
- Checkbox: 24px circle; done = solid `#4C6B4F` with white check; open = `1.6px solid #C6BDA9`
- Modals: centered, max-width 400–560px, scrim + `sheetIn`/`fadeIn` keyframes

## Interaction
- Hover states on every clickable (`style-hover`); pointer cursor everywhere clickable, incl. date inputs
- Min hit target 44px (52px FAB on mobile)
- Mobile breakpoint: <760px → filter chips row, compressed progress strip, FAB; `layout` prop forces `mobile`/`desktop`

## Reusability
- `Reminders.dc.html` is the single source app; Desktop/Tablet/Mobile files are thin wrappers via `dc-import` (Mobile wraps in `IOSDevice` from `ios-frame.jsx`)
- New screens: follow this file; copy patterns from `Reminders.dc.html` rather than restyling
