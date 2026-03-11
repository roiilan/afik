# Plan 014 — Status Cell Conditional Styling

## Goal

Apply conditional styling to the "Status" cell in the Exceptional Devices AG Grid table based on the device status value:

- **"critical" / "Critical"** → red background/text to indicate a severe warning
- **"warning" / "Warning"** → orange background/text to indicate a warning
- Other statuses remain unstyled (default)

The styling must only affect the specific "Status" cell of the matching row, not the entire row.

---

## Questions for User

### Q1: Styling approach — how should the status cell be visually distinguished? Option 3

- **Option 1:** Colored text only (e.g., red text for critical, orange text for warning)
- **Option 2:** Colored background with contrasting text (e.g., red background + white text for critical, orange background + dark text for warning)
- **Option 3:** Badge/pill style — small rounded label with colored background and text

**Recommendation:** Option 3 — a badge/pill style provides the clearest visual signal while keeping the table clean and professional. It's a common UI pattern for status indicators.

### Q2: Should we also style other statuses like "error" and "offline" that exist in the mock data? Option 2

- **Option 1:** Only style "critical" and "warning" as requested
- **Option 2:** Also add styles for "error" (e.g., dark red / different shade) and "offline" (e.g., gray)

**Recommendation:** Option 1 — stick to the exact requirements. Additional statuses can be added later if needed.

---

## Implementation Steps

1. **Add `cellStyle` callback to the Status column** in `Dashboard.jsx`:
   - Add a `cellStyle` function to the `status` column definition that checks the cell value (case-insensitive) and returns inline style for `critical` (red) and `warning` (orange).

2. **Add CSS classes for status cells** in `Dashboard.css`:
   - Alternatively, use AG Grid's `cellClass` or `cellClassRules` to apply CSS classes (`.status-critical`, `.status-warning`) instead of inline styles, keeping styling in CSS.

> **Note:** The exact implementation (inline `cellStyle` vs. CSS class via `cellClassRules`) depends on Q1 answer. For a badge/pill style, `cellClassRules` + CSS is cleaner. For simple color, `cellStyle` is sufficient.

---

## HISTORY

> Copilot: Do not use this section as implementation instructions.
> This section is for historical prompt documentation only.

<!--
- 2026-03-11: User requested Plan 014 — apply conditional styling to the "Status" cell in the Exceptional Devices AG Grid table. Critical status should be red, warning status should be orange. Styling should apply only to the specific Status cell, not the entire row.
- 2026-03-11: User chose Option 3 (badge/pill style) for Q1 and Option 1 (only critical & warning) for Q2. Plan approved.
- 2026-03-11: Implementation completed — added a cellRenderer to the status column in Dashboard.jsx that renders a span with CSS classes (status-badge + status-critical/status-warning) based on the lowercase value. Added corresponding badge/pill CSS in Dashboard.css with red (#e74c3c) for critical and orange (#f39c12) for warning.
- 2026-03-11: User changed Q1 answer from Option 3 (badge/pill) to Option 2 (colored background with contrasting text). Updated implementation to use AG Grid cellStyle callback instead of cellRenderer — applies full cell background color (red for critical, orange for warning, white text, bold). Removed badge/pill CSS classes.
- 2026-03-11: User changed answers again — Q1 to Option 1 (colored text only) and Q2 to Option 2 (also style error and offline). Updated cellStyle to text-only coloring: critical (#e74c3c red), warning (#f39c12 orange), error (#c0392b dark red), offline (#95a5a6 gray). No background color applied.
- 2026-03-11: User changed answers again — Q1 back to Option 3 (badge/pill) and Q2 stays Option 2 (all statuses). Switched from cellStyle to cellRenderer with badge/pill spans. Added CSS classes for all 4 statuses: critical (red), warning (orange), error (dark red), offline (gray).
-->
