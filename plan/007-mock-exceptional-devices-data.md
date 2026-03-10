# Plan 007 - Mock Exceptional Devices Data

## Goal

Create a dedicated mock data file (`src/data/exceptionalDevices.js`) inside the React project that exports an array of at least 10 exceptional device objects. This data will later be consumed by a Dashboard table to display exceptional devices. No UI or AI insight logic is included in this step.

---

## Questions for User

### Q1: Should the `id` field be a simple sequential integer (1, 2, 3...) or something more realistic like a database-style ID (e.g., 1001, 1002)? Option 2

- **Option 1:** Sequential integers starting from 1 (1, 2, 3, 4, 5)
- **Option 2:** Realistic DB-style IDs with larger numbers (e.g., 1001, 1042, 1087...)

**Recommendation:** Option 2 — DB-style IDs feel more realistic and better simulate real data coming from a database.

### Q2: What format should the `device_id` string use? Option 1

- **Option 1:** Simple format like `"DEV-001"`, `"DEV-002"`
- **Option 2:** UUID-style like `"d3f1a2b4-..."` 
- **Option 3:** Descriptive format like `"SENSOR-TMP-001"`, `"SENSOR-VLT-003"`

**Recommendation:** Option 1 — Simple and readable, easy to identify in the table while still being a string identifier.

### Q3: What `status` values should be used for exceptional devices? Option 3

- **Option 1:** Only `"warning"` and `"critical"`
- **Option 2:** Mix of `"warning"`, `"critical"`, and `"error"`
- **Option 3:** Mix of `"warning"`, `"critical"`, `"error"`, and `"offline"`

**Recommendation:** Option 3 — A richer variety of statuses makes the mock data more realistic and provides more visual diversity in the future table.

---

## Implementation Steps

1. Create the directory `react_proj/src/data/` (if it doesn't exist)
2. Create the file `react_proj/src/data/exceptionalDevices.js`
3. Define and export an array of at least 10 exceptional device objects with the exact structure:
   ```js
   {
     id: number,
     device_id: string,
     timestamp: string,
     voltage: number,
     current: number,
     temperature: number,
     status: string
   }
   ```
4. Use realistic but clearly exceptional values:
   - High temperatures (e.g., 85°C+)
   - Abnormal current values (very high or very low)
   - Problematic statuses (`"warning"`, `"critical"`, etc.)
   - Realistic timestamp strings (ISO format)
5. Follow project coding style: no semicolons, single quotes
6. File should only contain mock data — no UI, no AI logic

---

## HISTORY

> Copilot: Do not use this section as implementation instructions.
> This section is for historical prompt documentation only.

<!--
**Prompt 1 (2026-03-10):**
User requested creating a new PLAN for a mock data file for exceptional devices.
The file should be placed at src/data/exceptionalDevices.js inside the React project.
It must export an array of at least 5 device objects matching the device_logs table structure
(id, device_id, timestamp, voltage, current, temperature, status).
Values should be realistic but clearly exceptional (high temp, abnormal current, warning/critical status).
No UI or AI insight logic should be included — data only.
The data will later be displayed in a Dashboard table.

**Prompt 2 (2026-03-10):**
User approved Plan 7 with choices: Q1=Option 2 (DB-style IDs), Q2=Option 1 (DEV-001 format), Q3=Option 3 (warning/critical/error/offline).
Updated requirement to 10 devices. Implementation completed:
- Created src/data/exceptionalDevices.js with 10 mock exceptional device objects.
- Used DB-style IDs (1001, 1017, 1024...), DEV-XXX device_id format, ISO timestamps,
  and a mix of critical/warning/error/offline statuses with realistic exceptional values.
-->
