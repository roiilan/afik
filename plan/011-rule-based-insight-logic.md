# Plan 011 – Rule-Based Insight Logic

## Goal

Replace the placeholder logic in `backend/services/generateInsight.js` with real rule-based conditional logic that analyzes device fields (`temperature`, `current`, `voltage`) and returns a meaningful, deterministic explanation string for each exceptional device.

---

## Questions for User

### Q1: Threshold values for rules: - temperature: Option 2:
  - critical high: >= 85 °C
  - elevated: 60–84 °C
- current:
  - high: > 32 A
  - moderate: 16–32 A
  - low: < 6 A
- voltage:
  - low: < 200 V
  - normal: 200–250 V

Should the thresholds below be used for rule evaluation?

| Metric      | High     | Low      | Moderate     |
|-------------|----------|----------|--------------|
| temperature | > 85 °C  | —        | 50–85 °C     |
| current     | > 15 A   | —        | 5–15 A       |
| voltage     | < 200 V  | < 200 V  | —            |

- **Option 1** – Use exactly the thresholds above.
- **Option 2** – User provides custom thresholds.

**Recommendation:** Option 1 – These values align well with the mock data in `exceptionalDevices.js` and will trigger rules for most sample rows.

---

### Q2: Rule evaluation order — first match or accumulate? Option 1

- **Option 1** – Return the **first matching** rule's explanation (simple, deterministic).
- **Option 2** – Accumulate all matching rules and return a combined multi-line explanation.

**Recommendation:** Option 1 – First match keeps the output clean and straightforward; rules are ordered from most specific to least specific so the most relevant insight surfaces first.

---

### Q3: Number of rules — stick to 5 or add more? Option 1

- **Option 1** – Exactly 5 rules as listed in the requirements.
- **Option 2** – 5 core rules + 1–2 bonus rules (e.g., offline status, overvoltage).

**Recommendation:** Option 2 – Two extra rules (`offline` device detection and overvoltage) would cover more mock-data rows (DEV-009, DEV-018, DEV-031) and make the demo richer without adding complexity.

---

## Implementation Steps

1. Open `react_proj/backend/services/generateInsight.js`.
2. Define named threshold constants at the top of the file using the custom thresholds:
   - `CRITICAL_TEMP = 85` (critical high: >= 85 °C)
   - `ELEVATED_TEMP = 60` (elevated: 60–84 °C)
   - `HIGH_CURRENT = 32` (high: > 32 A)
   - `MODERATE_CURRENT = 16` (moderate: 16–32 A)
   - `LOW_CURRENT = 6` (low: < 6 A)
   - `LOW_VOLTAGE = 200` (low: < 200 V)
   - `MAX_NORMAL_VOLTAGE = 250` (normal: 200–250 V)
3. Implement a `switch(true)` block inside `generateInsight(device)` with first-match evaluation, rules ordered from most specific to least specific:
   - **Rule 1 – High temperature + high current:** `temperature >= 85 && current > 32` → "High temperature paired with high current may indicate cooling system failure or overheating under heavy load."
   - **Rule 2 – Critical status + high current:** `status === 'critical' && current > 32` → "Critical status with high current indicates severe overload or unstable operation requiring immediate attention."
   - **Rule 3 – Low voltage + high current:** `voltage < 200 && current > 32` → "Low voltage combined with high current suggests inefficient power delivery or electrical stress on the circuit."
   - **Rule 4 – High temperature + warning status:** `temperature >= 85 && status === 'warning'` → "Elevated temperature under warning status suggests poor ventilation or an early-stage thermal problem."
   - **Rule 5 – High current + normal voltage + elevated temperature:** `current > 32 && voltage >= 200 && voltage <= 250 && temperature >= 60 && temperature < 85` → "High current draw with normal voltage and elevated temperature may indicate abnormal downstream load behavior or power imbalance."
   - **Default:** "AI could not determine the root cause of the anomaly."
4. Verify the function remains a pure, deterministic function with no side effects.
5. Test manually by running the backend and calling the endpoint with mock device data.

---

## HISTORY

> Copilot: Do not use this section as implementation instructions.
> This section is for historical prompt documentation only.

<!--
- User requested a new plan to implement real rule-based logic inside `backend/services/generateInsight.js`.
- Current file contains a placeholder string return.
- The endpoint (`routes/aiInsights.js`) already parses and validates device fields from the query string and passes a typed device object to `generateInsight()`.
- User specified: use switch(true), at least 5 rules based on temperature/current/voltage/status combos, deterministic, readable, extensible.
- Plan created with 3 questions (thresholds, first-match vs accumulate, number of rules) and detailed implementation steps.
- User answered Q1: Option 2 – custom thresholds (temp: critical >=85, elevated 60-84; current: high >32, moderate 16-32, low <6; voltage: low <200, normal 200-250).
- User answered Q2: Option 1 – first match.
- User answered Q3: Option 1 – exactly 5 rules (no bonus rules).
- Implementation Steps updated to reflect the user's answers.
- User approved plan 11. Implementation completed: replaced placeholder in `generateInsight.js` with 7 named threshold constants and a `switch(true)` block containing 5 rules + default fallback.
- All 5 rules tested via HTTP requests to `localhost:3000/api/ai-insight` — each rule returned the expected insight string. Default fallback test was skipped by user but logic is trivial.
- User confirmed results and requested HISTORY update.
-->
