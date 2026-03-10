# Plan 008 - Dashboard with AG Grid Table

## Goal

Replace the default Vite starter UI with a clean Dashboard screen that displays the exceptional devices data in an AG Grid React table. This step focuses solely on building the table display — no AI insight logic or buttons.

---

## Questions for User

### Q1: What theme should the AG Grid table use? Option 1

- **Option 1:** AG Grid Quartz theme (modern, clean, built-in)
- **Option 2:** AG Grid Alpine theme (compact, professional)
- **Option 3:** AG Grid Balham theme (classic, data-dense)

**Recommendation:** Option 1 (Quartz) — it's the newest AG Grid theme with the most modern and polished look out of the box.

### Q2: Should the dashboard use a dark or light color scheme? Option 3

- **Option 1:** Dark theme only (matches the current Vite dark background)
- **Option 2:** Light theme only (clean, professional dashboard look)
- **Option 3:** Support both light and dark themes via CSS variables (as per cssLayer skill)

**Recommendation:** Option 3 — the cssLayer skill requires light/dark theme support via CSS variables. We should set this up properly from the start.

### Q3: How should the timestamp be formatted? Option 2

- **Option 1:** `Mar 10, 2026, 08:15 AM` (US-style, 12-hour)
- **Option 2:** `10/03/2026 08:15:32` (DD/MM/YYYY, 24-hour)
- **Option 3:** `2026-03-10 08:15:32` (ISO-like, 24-hour)

**Recommendation:** Option 1 — more human-readable and friendlier for a dashboard UI.

### Q4: Should we set up the full CSS folder structure from the cssLayer skill in this step? Option 1

- **Option 1:** Yes — create the full `style/` folder structure (setup, basics, cmps) now
- **Option 2:** No — create only the files needed for this step, set up the full structure later

**Recommendation:** Option 1 — it's better to establish the CSS architecture early so all subsequent steps build on a solid foundation.

---

## Implementation Steps

1. **Install AG Grid React** — run `npm install ag-grid-react ag-grid-community` in `react_proj/`
2. **Set up CSS folder structure** (per cssLayer skill):
   - Create `react_proj/src/style/main.css` — imports all sub-files
   - Create `react_proj/src/style/setup/var.css` — CSS variables for colors (light + dark)
   - Create `react_proj/src/style/setup/typography.css` — font settings
   - Create `react_proj/src/style/basics/reset.css` — CSS reset
   - Create `react_proj/src/style/basics/helper.css` — utility classes
   - Create `react_proj/src/style/basics/layout.css` — layout utilities
   - Create `react_proj/src/style/basics/base.css` — base element styles
   - Create `react_proj/src/style/basics/button.css` — button styles (minimal for now)
   - Create `react_proj/src/style/cmps/Dashboard.css` — Dashboard component styles
3. **Update `main.jsx`** — import `style/main.css` instead of `index.css`
4. **Rewrite `App.jsx`** — remove Vite starter content, render a `<Dashboard />` component
5. **Create `Dashboard.jsx`** component:
   - Import mock data from `src/data/exceptionalDevices.js`
   - Import and configure `AgGridReact` with `AllCommunityModule` via `ModuleRegistry`
   - Define column definitions for: ID, Device ID, Timestamp, Voltage, Current, Temperature, Status
   - Format the Timestamp column using a `valueFormatter`
   - Show a friendly empty state message ("No exceptional devices found")
   - Wrap the grid in a card-like container
6. **Clean up old files** — remove/empty `App.css` and `index.css` (replaced by style/ structure)

---

## HISTORY

> Copilot: Do not use this section as implementation instructions.
> This section is for historical prompt documentation only.

<!--
- 2026-03-10: User requested Plan 008 — replace the default Vite starter UI with a Dashboard screen using AG Grid React to display exceptional devices data. Requirements include: AG Grid Community setup with AgGridProvider + AllCommunityModule, timestamp formatting, polished modern UI, card container, empty state handling. No AI insight button or logic in this step. CSS must follow the cssLayer skill (plain CSS, folder structure, light/dark theme support via variables).
- 2026-03-10: User approved Plan 008 with answers: Q1=Option 1 (Quartz theme), Q2=Option 3 (light+dark via CSS vars), Q3=Option 2 (DD/MM/YYYY 24h), Q4=Option 1 (full CSS structure now).
- 2026-03-10: Implementation completed:
  - Installed ag-grid-react and ag-grid-community
  - Created full CSS folder structure: style/main.css, setup/ (var.css, typography.css), basics/ (reset.css, base.css, layout.css, helper.css, button.css), cmps/ (Dashboard.css)
  - Light and dark theme support via CSS variables with prefers-color-scheme media query
  - Updated main.jsx to import style/main.css
  - Rewrote App.jsx to render Dashboard component inside a main-layout wrapper
  - Created src/cmps/Dashboard.jsx with AG Grid React (Quartz theme, AllCommunityModule, DD/MM/YYYY timestamp format, empty state handling, card layout)
  - Cleared old App.css and index.css
  - Build verified successfully
-->
