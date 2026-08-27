# Product Brief

## Product statement

The product is a single, persistent desktop checklist that minimizes the effort between remembering something and recording it. Users write and check; automatic ordering and completion retention stay in the background. It is local-only and requires no account or network connection.

The product brand is **妥了 LiteTick**.

## Core behavior

### macOS
1. A circular green floating ball with one centered white checkmark appears near the right edge by default without covering active work; the user can drag it anywhere within the current screen.
2. Hovering expands the floating panel immediately.
3. The panel opens in a neutral browsing state; clicking the input begins capture.
4. Return saves the item at the top of the unfinished list.
5. Checking an item removes it from the main list and places it in the Completed view.
6. A completed item shows a green checked square in Completed. Clicking that control returns it to the unfinished list.
7. Completed provides search, individual permanent deletion, and confirmed deletion of all completed items.
8. Moving the pointer away collapses an unengaged panel after a short delay. Clicking or typing keeps it open; `×` or clicking another application dismisses it. A session-only toolbar pin keeps it above applications and across Spaces.
9. Unfinished items can be reordered by dragging.
10. Item text can be edited with a single click or the context menu; Return or clicking outside saves the change.
11. The main panel contains unfinished items only; all completed items live in Completed.
12. Permanent deletion requires confirmation.
13. No global keyboard shortcut is included.
14. The panel includes a visible globe language switch and remembers the selected language.
15. The panel supports light and dark appearances. Light is the default; a single bilingual sun/moon control beside the language switch toggles directly between the two modes and remembers the choice.
16. A Hover-only panel auto-collapses after pointer exit. Clicking or typing engages it; `×` or an external left/right click dismisses it and leaves the floating trigger running. The application quits from the trigger's bilingual context menu or `Command-Q`, with a native confirmation.

### Windows (post-v1)
1. Preserve the same single-list model and local-only privacy boundary.
2. Adapt lightweight capture to native Windows interaction conventions.
3. Require no account, server, or synchronization.

## Data principle

An item needs text, completion state, ordering information, and system-managed timestamps. Dates are retained for persistence and history but are not presented as a planning system in the main interface.

## Visual principle

- Simple, calm, and work-focused.
- Simplified Chinese and English are first-class UI languages.
- Current primary-green candidate: `#07C160`.
- Neutral semantic system surfaces carry the interface; green is reserved for brand, completion, focus, and primary actions. Dark mode avoids pure black and uses a brighter accessible green variant.
- Familiar international component behavior, accessible contrast, keyboard support, sufficient touch targets, and reduced-motion support.
- Inspiration from familiar green communication products is limited to color familiarity and restraint; icons and layouts must remain original.

## MVP boundary

The MVP does not include categories, tags, folders, calendars, deadlines, reminders, priorities, progress bars, timelines, voice input, AI features, analytics, complex formatting, accounts, servers, or device synchronization. Official development targets macOS first and Windows later; other platforms are left to community contributors.

## Current assets

- `design/brand/litetick-app-icon.svg`
- `design/brand/litetick-alert-icon.svg`
