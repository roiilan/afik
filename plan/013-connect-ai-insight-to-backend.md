# Plan 013 — Connect AI Insight Column to Backend Endpoint

## Goal

Wire the existing "Generate AI Insights" button in the AG Grid AI Insight column to the real backend endpoint (`GET /api/ai-insight`). On click, send only the 4 fields needed by the rule logic (`temperature`, `current`, `voltage`, `status`), show loading while in-flight, display the returned insight text on success, and restore the button on failure.

---

## Questions for User

### Q1: Backend — reduce required fields to only 4? Option B

The backend route currently validates and requires **all 7 fields** (`id`, `device_id`, `timestamp`, `voltage`, `current`, `temperature`, `status`), but `generateInsight()` only uses 4 of them (`temperature`, `current`, `voltage`, `status`).

Since the task says to send **only the fields actually required by the rule logic**, the backend validation needs to be updated too.

- **Option A** — Update the backend `REQUIRED_FIELDS` to `['voltage', 'current', 'temperature', 'status']` only, removing the unused fields from validation.
- **Option B** — Keep all 7 fields required on the backend and send all 7 from the frontend anyway.

**Recommendation:** Option A — keeps the API contract honest and aligned with the actual logic. No reason to require fields that are never used.

---

### Q2: Error message display style - Option 1

When a request fails, the task says to optionally show a short inline error message like *"Failed to generate AI insight. Please try again."*

- **Option 1** — Show the error text briefly above/below the restored button inside the same cell, then the user can click the button again.
- **Option 2** — Show only the restored button (no visible error text), relying on the button reappearance as the retry signal.

**Recommendation:** Option 1 — gives the user clear feedback about what happened while still allowing retry.

---

### Q3: Backend base URL configuration - Option 2 — a small service file (`src/services/aiInsightService.js`) keeps the fetch logic out of the component and makes the base URL easy to change later.

The backend runs on `http://localhost:3000`. Where should this URL be defined?

- **Option 1** — Define it as a constant at the top of `AiInsightCell.jsx`.
- **Option 2** — Define it in a shared config/constants file (e.g. `src/config.js` or `src/services/aiInsightService.js`).
- **Option 3** — Use a Vite environment variable (`import.meta.env.VITE_API_URL`).

**Recommendation:** Option 2 — a small service file (`src/services/aiInsightService.js`) keeps the fetch logic out of the component and makes the base URL easy to change later.

---

## Implementation Steps

> Will be finalized after user answers the questions above.

**Planned steps (pending approval):**

1. **Update backend route** — Change `REQUIRED_FIELDS` to `['voltage', 'current', 'temperature', 'status']` and remove unused fields from validation/parsing (if Q1 → Option A).

2. **Create frontend service** — Add `src/services/aiInsightService.js` with a `fetchAiInsight({ temperature, current, voltage, status })` function that builds the query string with `URLSearchParams` and calls the endpoint via `fetch` (if Q3 → Option 2).

3. **Update `AiInsightCell.jsx`** — 
   - Accept AG Grid `params` (the component receives `params.data` automatically as a cell renderer).
   - On button click: call the service, show loading, on success show `response.insight`, on error restore the button + show inline error text.
   - Add a `'done'` and `'error'` status to the existing state machine.

4. **Update `Dashboard.css`** — Add styles for the insight text (multi-line wrapping, readable) and the inline error message.

5. **Manual testing** — Verify with backend running: success flow, error flow (server stopped), retry after failure.

---

## HISTORY

> Copilot: Do not use this section as implementation instructions.
> This section is for historical prompt documentation only.

<!--
- User requested connecting the existing AI Insight column button to the backend endpoint.
- Current state: AG Grid table with "Generate AI Insights" button per row, button hides and shows loading spinner on click, but no actual API call is made.
- Backend endpoint exists at GET /api/ai-insight and works, but currently requires all 7 device fields while only 4 are used by the rule logic.
- Plan created with 3 questions: reducing backend required fields, error display style, and API base URL configuration.
- User answered: Q1 → Option B (keep all 7 fields), Q2 → Option 2 (no error text, just restore button), Q3 → Option 2 (service file).
- Implementation completed:
  1. Created `react_proj/frontend/src/services/aiInsightService.js` — fetch wrapper using URLSearchParams, sends all 7 fields.
  2. Updated `AiInsightCell.jsx` — component now accepts `{ data }` from AG Grid, calls the service on click, shows insight text on success, restores button on failure.
  3. Updated `Dashboard.css` — added `.ai-insight-text` class for multi-line wrapping and readable text inside the cell.
- User changed Q2 answer from Option 2 to Option 1 (show inline error message). Requested: on failure, show error message for 10 seconds, then restore the button.
- Implementation updated:
  1. `AiInsightCell.jsx` — added `error` status state. On fetch failure, shows "Failed to generate AI insight. Please try again." for 10 seconds, then reverts to idle (button restored). Uses `useRef` + `useEffect` cleanup for the timer.
  2. `Dashboard.css` — added `.ai-insight-error` class with red text styling.
-->
