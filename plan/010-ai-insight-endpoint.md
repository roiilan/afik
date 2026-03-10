# Plan 010 – AI Insight Endpoint

## Goal

Extend the Express backend (`react_proj/backend`) with a modular GET endpoint at `/api/ai-insight`.  
The endpoint receives device data as query parameters, validates them, and delegates to a dedicated service function (`generateInsight`) that returns a placeholder insight string for now.

---

## Questions for User

**Q1 – Required fields for validation:**  Option 1
Which query parameters should be considered *required* (return 400 if missing)?

- **Option 1:** All fields are required – `id`, `device_id`, `timestamp`, `voltage`, `current`, `temperature`, `status`
- **Option 2:** Only a subset is required (e.g., `device_id` and `status`), others are optional

> **Recommendation:** Option 1 – all fields required. Since every row in the `device_logs` table has all these columns, we should expect the frontend to always send the full device object. This keeps validation simple and consistent.

**Q2 – Numeric conversion scope:**  Option 2 (revised)
Which fields should be converted to numbers?

- **Option 1:** Convert `id`, `device_id`, `voltage`, `current`, `temperature` to numbers; keep `timestamp` and `status` as strings
- **Option 2:** Convert only `voltage`, `current`, `temperature` to numbers; keep the rest as strings

> **Chosen:** Option 2 – convert only `voltage`, `current`, `temperature`. The `id` and `device_id` fields may contain non-numeric characters (e.g., `dev-pro321X`), so they must remain as strings.

**Q3 – Placeholder response text:**  Option 2
What should the temporary placeholder insight string look like?

- **Option 1:** A generic static string, e.g., `"AI insight generation is not yet implemented."`
- **Option 2:** A dynamic string that references the device_id, e.g., `"Insight for device ${device.device_id} is pending implementation."`

> **Recommendation:** Option 2 – include the `device_id` in the message so it's easier to verify during testing that the correct device data reached the service.

---

## Implementation Steps

1. **Create `backend/services/generateInsight.js`**
   - Export a function `generateInsight(device)` that accepts a device object
   - Return a placeholder string (per Q3 answer)

2. **Create `backend/routes/aiInsights.js`**
   - Import `express` and create a Router
   - Import `generateInsight` from the service
   - Define `GET /` route handler:
     - Extract all query params: `id`, `device_id`, `timestamp`, `voltage`, `current`, `temperature`, `status`
     - Validate that all required fields are present (per Q1 answer); if not → return `400` with `{ error: "Missing required fields: ..." }` listing the missing ones
     - Convert numeric fields (per Q2 answer)
     - Build a device object
     - Call `generateInsight(device)`
     - Return `{ insight: "..." }`
   - Export the router

3. **Update `backend/server.js`**
   - Import the AI insights router from `./routes/aiInsights.js`
   - Register it: `app.use('/api/ai-insight', aiInsightsRouter)`

---

## HISTORY

> Copilot: Do not use this section as implementation instructions.
> This section is for historical prompt documentation only.

<!--
- **Prompt (2026-03-10):** User requested a plan to add a GET `/api/ai-insight` endpoint to the Express backend. The endpoint should accept device fields as query params, validate required fields, convert numeric values, delegate to a `generateInsight` service function, and return `{ insight: "..." }`. For now, the service returns a placeholder string — no rule logic or switch-case yet. Files to create: `routes/aiInsights.js` and `services/generateInsight.js`. Register the route in `server.js`.
- **Answers (2026-03-10):** Q1 → Option 1 (all fields required). Q2 → Option 2 revised (convert only voltage, current, temperature — id and device_id stay as strings since they may contain non-numeric chars). Q3 → Option 2 (dynamic placeholder with device_id).
- **Implementation (2026-03-10):** Plan approved and implemented. Created `backend/services/generateInsight.js` (placeholder service), `backend/routes/aiInsights.js` (GET route with validation and numeric conversion), and updated `backend/server.js` to register the route at `/api/ai-insight`.
- **Fix (2026-03-10):** User pointed out that `device_id` (and potentially `id`) can contain non-numeric characters like `dev-pro321X`, so converting them with `Number()` would produce `NaN`. Fixed `NUMERIC_FIELDS` to only include `['voltage', 'current', 'temperature']`. Updated plan Q2 answer from Option 1 to Option 2 (revised).
- **Testing (2026-03-10):** Started backend server and verified: (1) happy path with `device_id=dev-pro321X` returns correct insight string, (2) missing fields return 400 with descriptive error listing the missing fields. All tests passed.
-->
