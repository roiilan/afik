# Plan 012 — AI Insight Column UI (Button + Loading State)

## Goal

Add a new "AI Insight" column to the existing AG Grid table in the Dashboard. The column displays a "Generate AI Insights" button per row. When clicked, the button disappears and a spinning loader appears in its place. No backend call or insight text is returned in this step — only the UI behavior and loading state are implemented.

---

## Questions for User

### Q1: Button style — which approach? Option 1
- **Option 1:** Use the existing `.btn .btn-primary` style (accent-colored solid button)
- **Option 2:** Use a subtle outlined/ghost button (border only, transparent background) that is less visually heavy inside the grid
- **Option 3:** Use a small compact accent button with reduced padding to fit neatly inside the row

**Recommendation:** Option 3 — a compact accent button keeps the button visible and actionable while fitting cleanly inside an AG Grid row without feeling oversized.

### Q2: Loading indicator style? Option 1
- **Option 1:** Pure CSS spinning gear icon (SVG gear with CSS `@keyframes rotate`)
- **Option 2:** Simple CSS spinner ring (border-based rotating circle) — lightweight and fits most UIs
- **Option 3:** Pulsing dots animation

**Recommendation:** Option 1 — a spinning gear icon signals "AI processing" effectively and matches the request. We'll use a small inline SVG gear with a CSS rotation animation.

### Q3: Column width for "AI Insight"? Option 3
- **Option 1:** Fixed width (e.g. 200px)
- **Option 2:** `flex: 1` so it takes remaining space
- **Option 3:** `minWidth: 200, flex: 1` — flexible but with a minimum

**Recommendation:** Option 3 — this ensures the column is wide enough for the button/loader now and for multi-line text later, while adapting to available space.

### Q4: Row height — should we increase it to prepare for multi-line insight text? Option 2
- **Option 1:** Keep the default AG Grid row height for now; increase it later when insight text is added
- **Option 2:** Set `autoHeight: true` on the AI Insight column now so it adapts automatically
- **Option 3:** Set a slightly taller fixed row height (e.g. 48px) as a middle ground

**Recommendation:** Option 2 — setting `autoHeight: true` on the AI Insight column now means when insight text is added later, the row will expand naturally. For the button/loader state it won't change appearance.

---

## Implementation Steps

1. **Create a custom AG Grid cell renderer component** (`AiInsightCell.jsx`) in `react_proj/frontend/src/cmps/`:
   - Accept row data via AG Grid `params`
   - Manage local state: `idle` (show button) and `loading` (show spinner)
   - On button click → set state to `loading`, button disappears, spinner appears
   - Render a compact accent-styled button in `idle` state
   - Render a spinning gear SVG loader in `loading` state

2. **Add the "AI Insight" column** to `Dashboard.jsx`:
   - Add a new column definition after `status` with `headerName: 'AI Insight'`
   - Set `cellRenderer: AiInsightCell`
   - Set `minWidth: 200, flex: 1, autoHeight: true`
   - Set `sortable: false, resizable: false` (not meaningful for this column)

3. **Add CSS for the AI Insight cell** in `Dashboard.css`:
   - `.ai-insight-btn` — compact accent button style
   - `.ai-insight-loader` — spinning gear animation
   - `@keyframes spin` — rotation animation
   - Ensure styles work in both light and dark themes using existing CSS variables

---

## HISTORY

> Copilot: Do not use this section as implementation instructions.
> This section is for historical prompt documentation only.

<!--
- 2026-03-10: User requested Plan 012 — add a new "AI Insight" column to the AG Grid table in Dashboard. The column should show a "Generate AI Insights" button per row. On click, the button disappears and a spinning gear loader appears. No backend call or insight text yet — only UI behavior and loading state. Must use AG Grid best practices (custom cell renderer), keep existing columns unchanged, and prepare the cell for later multi-line text display.
- 2026-03-10: User approved Plan 012 with choices: Q1 Option 1 (existing .btn .btn-primary style), Q2 Option 1 (spinning gear SVG), Q3 Option 3 (minWidth 200 + flex 1), Q4 Option 2 (autoHeight true). Implementation completed:
  - Created AiInsightCell.jsx — custom cell renderer with idle/loading state and inline gear SVG spinner.
  - Added AI Insight column to Dashboard.jsx after Status column with cellRenderer, minWidth, flex, autoHeight, sortable/resizable false.
  - Added CSS in Dashboard.css for .ai-insight-cell, .ai-insight-btn, .ai-insight-spinner, and @keyframes spin.
- 2026-03-10: User requested UI fixes: (1) increase button font size, (2) set min-height on rows so they don't shrink when button disappears, (3) replace gear spinner with hourglass spinner. Applied: font-size changed from fs-xs to fs-sm, min-height set to 34px on .ai-insight-cell, replaced GearSpinner SVG with HourglassSpinner SVG, kept spin animation.
- 2026-03-10: User requested: (1) set minimum row height to 65px, (2) center the loading icon in the cell, (3) slow down the spin animation. Applied: added rowHeight={65} to AgGridReact, updated min-height to 65px and added justify-content: center on .ai-insight-cell, slowed animation from 1.2s to 2.5s.
- 2026-03-10: User requested centering the "AI Insight" column header text. Applied: added headerClass 'ag-header-cell-center' to the column definition and a CSS rule to center the header label.
-->
