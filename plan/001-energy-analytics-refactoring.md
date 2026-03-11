# Plan 001 - EnergyAnalyticsService Refactoring

## Goal

Analyze the `EnergyAnalyticsService.cs` file, explain what it does with examples, and perform a full refactoring based on best practices, clean code, and performance improvements.

---

## Part 1: Code Analysis - What does EnergyAnalyticsService do?

### Overview

The `EnergyAnalyticsService` class contains a single method `CalculateEfficiencyMetrics` that receives a list of raw energy readings (`RawData`) from devices and aggregates them into per-device results (`DeviceResult`).

### Step-by-step breakdown

1. **Input**: A `List<RawData>` where each item represents a single reading from a device, containing:
   - `DeviceId` - identifies the device
   - `Voltage` - voltage reading (V)
   - `Current` - current reading (A)
   - `Temperature` - temperature reading

2. **Power Calculation** (line 14):
   ```
   powerUsage = Voltage * Current
   ```
   This is the standard electrical power formula: P = V × I

3. **Efficiency Factor Calculation** (line 16):
   ```
   efficiencyFactor = Math.Pow(Math.Sqrt(powerUsage * 0.85), 2) / (Temperature + 1)
   ```
   **Note**: `Math.Pow(Math.Sqrt(x), 2)` simplifies to just `x`. So this is equivalent to:
   ```
   efficiencyFactor = (powerUsage * 0.85) / (Temperature + 1)
   ```
   This is a redundant mathematical operation - a key refactoring target.

4. **Aggregation** (lines 18-35): For each reading, it either:
   - Finds an existing `DeviceResult` for this device and updates it
   - Or creates a new `DeviceResult`

5. **Output**: A `List<DeviceResult>` with per-device aggregated metrics.

### Example

**Input Data:**
```
Device "A": Voltage=220, Current=5, Temperature=25
Device "B": Voltage=110, Current=3, Temperature=30
Device "A": Voltage=225, Current=4.8, Temperature=27
```

**Processing:**

| Step | DeviceId | Power (V×I) | Efficiency Factor | Action |
|------|----------|-------------|-------------------|--------|
| 1 | A | 220×5 = 1100 | (1100×0.85)/(25+1) = 35.96 | Create new result |
| 2 | B | 110×3 = 330 | (330×0.85)/(30+1) = 9.05 | Create new result |
| 3 | A | 225×4.8 = 1080 | (1080×0.85)/(27+1) = 32.79 | Update existing: TotalPower=2180, Count=2, AvgEff=(35.96+32.79)/2=34.37 |

**Output:**
```
DeviceId="A", TotalPower=2180, ReadingsCount=2, AverageEfficiency=34.37
DeviceId="B", TotalPower=330, ReadingsCount=1, AverageEfficiency=9.05
```

---

## Part 2: Issues Found & Refactoring Checklist

### Logic Bugs

- [x] **1. Redundant math operation**: `Math.Pow(Math.Sqrt(x), 2)` = `x`. Simplified to `(powerUsage * EfficiencyCoefficient) / (item.Temperature + 1)`.

- [x] **2. Incorrect average calculation**: Replaced broken formula with `EfficiencySum` accumulator + computed property `AverageEfficiency => EfficiencySum / ReadingsCount`.

- [x] **3. Magic number `0.85`**: Extracted to `private const double EfficiencyCoefficient = 0.85`.

- [x] **4. Division by zero / near-zero denominator**: Replaced exact `== 0` check with `Math.Abs(item.Temperature + 1) < MinDenominatorThreshold` (`1e-6`). Skipped readings exposed via `IReadOnlyList<SkippedReading> LastSkippedReadings` on the service (non-breaking). `LastSkippedReadings` resets on every call.

### Performance Issues

- [x] **5. O(n×m) lookup with `FirstOrDefault`**: Replaced with `Dictionary<string, DeviceResult>` for O(1) lookups.

### Clean Code & Best Practices

- [x] **6. Input validation**: Added `ArgumentNullException` guard clause for null `data` parameter.

- [x] **7. Nullable reference types**: All string properties now default to `string.Empty`.

- [ ] **8. Separation of concerns**: User chose to keep all classes in one file (Option 1). No change needed.

- [x] **9. Naming**: Internal variables are clear and descriptive. No change needed.

- [x] **10. Immutability**: `AverageEfficiency` is now a computed read-only property. `EfficiencySum` has a `private set` — cannot be written from outside `DeviceResult`. Updates go through `AccumulateReading(power, efficiency)` method.

### Additional Fixes (from Reflection Round — Points 2–5)

- [x] **11. Null item in list**: A null element inside the `data` list would throw `NullReferenceException`. Added explicit null-item check at the top of the loop. Skipped with reason `"Reading is null"`, `DeviceId` set to `string.Empty`.

- [x] **12. DeviceId null/empty/whitespace**: A null `DeviceId` would crash `Dictionary.TryGetValue` (`ArgumentNullException`). An empty or whitespace `DeviceId` would silently group all such readings under a meaningless key. Added `string.IsNullOrWhiteSpace` check. Skipped with reason `"DeviceId is null, empty, or whitespace"`.

- [x] **13. NaN/Infinity in numeric fields**: `double.NaN` or `double.Infinity` in `Voltage`, `Current`, or `Temperature` would silently corrupt `TotalPower` and `EfficiencySum` without throwing. NaN also bypasses the denominator guard (`NaN == 0` is false). Added `double.IsFinite()` check on all three fields. Skipped with reason describing which fields are affected.

- [x] **14. Near-zero denominator guard**: Upgraded from exact `== 0` to `Math.Abs(item.Temperature + 1) < MinDenominatorThreshold` (`1e-6`). Prevents extreme efficiency values from near-zero denominators, not just exact zero.

---

## Questions for User

1. **What does the `0.85` magic number represent?** Is it an efficiency coefficient, a power factor, or something else? This will help us name the constant properly.
 Use EfficiencyCoefficient. The exact business meaning of 0.85 was not provided, so I prefer a neutral and descriptive name.

2. **Should we separate the model classes (`RawData`, `DeviceResult`) into their own files?** option 1
   - Option 1: Keep all in one file (simpler for small projects)
   - Option 2: Separate each class into its own file (standard C# convention) **(Recommended)** 

3. **Should we add Temperature validation?** 
Temperature can be any numeric value, including negative values, so negative temperature should not be treated as invalid by itself.Please update this item accordingly:
- do not reject a reading only because Temperature is negative
- if `Temperature + 1 == 0`, skip that reading
- keep track of skipped readings so they are visible and not silently lost
- continue processing the rest of the batch
   <!-- - Option 1: Throw an `ArgumentException` if Temperature == -1
   - Option 2: Skip readings with invalid temperature and log a warning **(Recommended)**
   - Option 3: Clamp temperature to a minimum of 0 -->

4. **Should the refactored code use LINQ grouping for the aggregation instead of the manual foreach loop?** option 1
   - Option 1: Keep foreach with Dictionary (imperative, clear) **(Recommended)**
   - Option 2: Use LINQ GroupBy (functional, more concise)

---

## Implementation Steps

Status: **COMPLETED**

All items implemented in a single pass:

1. ~~Fix the redundant math operation (item 1)~~ - Done
2. ~~Fix the incorrect average calculation (item 2)~~ - Done
3. ~~Extract magic number to named constant (item 3)~~ - Done
4. ~~Add temperature validation / division-by-zero guard (item 4)~~ - Done
5. ~~Replace FirstOrDefault with Dictionary for O(1) lookup (item 5)~~ - Done
6. ~~Add input validation / guard clauses (item 6)~~ - Done
7. ~~Improve model classes - nullable annotations (items 7, 10)~~ - Done
8. Item 8 (file separation) - Skipped per user preference (Option 1)

### What Changed

- **Return type** kept as `List<DeviceResult>` (no breaking change)
- **New service property**: `IReadOnlyList<SkippedReading> LastSkippedReadings` — populated after every call, reset on every call
- **`EfficiencyResult` class removed** (was introduced in the previous iteration, callers had not adopted it)
- **`SkippedReading` class added** (referenced by `LastSkippedReadings`)
- **`DeviceResult`** now has `EfficiencySum` (internal accumulator) and `AverageEfficiency` (computed property)
- **`AverageEfficiency`** is no longer a settable property — it is computed as `EfficiencySum / ReadingsCount`

### What Did Not Change

- Method signature `List<DeviceResult> CalculateEfficiencyMetrics(List<RawData> data)` — kept as-is
- Method name `CalculateEfficiencyMetrics` kept as-is
- All classes remain in the same file
- Business logic intent preserved (power = V × I, efficiency = power × 0.85 / (temp + 1))

---

## Part 3: Verification — Fake Data & Edge Cases

All cases traced manually against the final implementation.

---

### Case 1: Correct running average — 3 readings, same device

**Input:**
```
{DeviceId="A", V=100, C=2, T=9}
{DeviceId="A", V=150, C=2, T=9}
{DeviceId="A", V=200, C=2, T=9}
```

**Trace:**

| Step | T+1 | power | eff (power×0.85/T+1) | deviceMap["A"] |
|------|-----|-------|----------------------|----------------|
| 1 | 10 | 200 | 17.0 | TotalPower=200, Count=1, Sum=17.0 |
| 2 | 10 | 300 | 25.5 | TotalPower=500, Count=2, Sum=42.5 |
| 3 | 10 | 400 | 34.0 | TotalPower=900, Count=3, Sum=76.5 |

**Result:** AverageEfficiency = 76.5 / 3 = **25.5** ✓

**Old broken formula would have given:** step2=(17+25.5)/2=21.25, step3=(21.25+34)/2=**27.625** ✗

---

### Case 2: Temperature = -1 (division by zero — skip)

**Input:**
```
{DeviceId="B", V=100, C=2, T=-1}
```

**Trace:** T+1 = 0 → **SKIP**

**Result:**
- `DeviceResults` = `[]`
- `LastSkippedReadings` = `[{DeviceId="B", Reason="..."}]` ✓

---

### Case 3: Negative temperature that is NOT -1 (valid reading)

**Input:**
```
{DeviceId="C", V=100, C=2, T=-5}
```

**Trace:** T+1 = -4 ≠ 0 → process. power=200. eff = 200×0.85 / (-4) = **-42.5**

**Result:**
- `DeviceResults` = `[{DeviceId="C", TotalPower=200, ReadingsCount=1, AverageEfficiency=-42.5}]`
- `LastSkippedReadings` = `[]`
- Negative efficiency is an unusual domain result but the code does not reject it — per requirement ✓

---

### Case 4: Empty input list

**Input:** `data = []`

**Trace:** No iterations. `deviceMap = {}`, `skippedReadings = []`

**Result:**
- Returns `[]`
- `LastSkippedReadings` = `[]` ✓

---

### Case 5: Null input

**Input:** `data = null`

**Trace:** Guard clause fires immediately.

**Result:** `ArgumentNullException` thrown ✓

---

### Case 6: Mixed batch — some valid, one skipped

**Input:**
```
{DeviceId="A", V=220, C=5, T=9}
{DeviceId="B", V=100, C=2, T=-1}
{DeviceId="A", V=180, C=4, T=9}
```

**Trace:**

| Step | DeviceId | T+1 | power | eff | Action |
|------|----------|-----|-------|-----|--------|
| 1 | A | 10 | 1100 | 93.5 | Create A |
| 2 | B | 0 | — | — | **SKIP** |
| 3 | A | 10 | 720 | 61.2 | Update A: TotalPower=1820, Count=2, Sum=154.7 |

**Result:**
- `DeviceResults` = `[{DeviceId="A", TotalPower=1820, ReadingsCount=2, AverageEfficiency=77.35}]`
- `LastSkippedReadings` = `[{DeviceId="B"}]` ✓

---

### Case 7: All readings skipped

**Input:**
```
{DeviceId="A", V=100, C=2, T=-1}
{DeviceId="B", V=50,  C=3, T=-1}
```

**Trace:** Both T+1=0 → both skipped.

**Result:**
- `DeviceResults` = `[]`
- `LastSkippedReadings` = `[{DeviceId="A"}, {DeviceId="B"}]` ✓

---

### Case 8: Zero power (V=0)

**Input:**
```
{DeviceId="X", V=0, C=5, T=9}
```

**Trace:** T+1=10. power=0. eff=0×0.85/10=0.

**Result:**
- `DeviceResults` = `[{DeviceId="X", TotalPower=0, ReadingsCount=1, AverageEfficiency=0}]`
- No divide-by-zero, no crash ✓

---

### Case 9: Multiple calls — LastSkippedReadings resets between calls

**Call 1:** `[{DeviceId="A", T=-1}]`
→ `DeviceResults=[]`, `LastSkippedReadings=[{A}]`

**Call 2:** `[{DeviceId="B", V=100, C=2, T=9}]`
→ Inside call: `skippedReadings = new List<SkippedReading>()` (fresh allocation)
→ No skips
→ `LastSkippedReadings = skippedReadings` (the empty list)

**Result after Call 2:**
- `DeviceResults` = `[{DeviceId="B", ...}]`
- `LastSkippedReadings` = `[]` — stale data from Call 1 is gone ✓

---

### Summary

| Case | Description | Result |
|------|-------------|--------|
| 1 | Correct running average | PASS ✓ |
| 2 | Temperature = -1 → skip | PASS ✓ |
| 3 | Negative temp ≠ -1 → valid | PASS ✓ |
| 4 | Empty input | PASS ✓ |
| 5 | Null input | PASS ✓ |
| 6 | Mixed batch | PASS ✓ |
| 7 | All readings skipped | PASS ✓ |
| 8 | Zero power | PASS ✓ |
| 9 | LastSkippedReadings resets each call | PASS ✓ |

---

## Part 4: Additional Fixes — Reflection Round

Fixes implemented based on the reflection review (Points 2–5). All are skip-and-track; the method signature and return type remain unchanged.

### Guard order in the loop (top to bottom)

Each reading now passes through 4 guards before any calculation is attempted:

```
1. item == null                                → skip, DeviceId=""
2. string.IsNullOrWhiteSpace(item.DeviceId)   → skip
3. !double.IsFinite(Voltage/Current/Temp)     → skip
4. Math.Abs(Temperature + 1) <= 1e-6          → skip
   ↓
   Safe to calculate: powerUsage, efficiencyFactor
```

### Constants added

| Constant | Value | Purpose |
|----------|-------|---------|
| `MinDenominatorThreshold` | `1e-6` | Near-zero denominator guard threshold |

### Edge cases now covered

| Scenario | Guard | Reason in SkippedReading |
|----------|-------|--------------------------|
| `null` element in list | Guard 1 | `"Reading is null"` |
| DeviceId is `null` | Guard 2 | `"DeviceId is null, empty, or whitespace"` |
| DeviceId is `""` or `" "` | Guard 2 | `"DeviceId is null, empty, or whitespace"` |
| `Voltage = double.NaN` | Guard 3 | `"One or more numeric fields ... contain NaN or Infinity"` |
| `Temperature = double.PositiveInfinity` | Guard 3 | same |
| `Temperature = -1.0` (exact) | Guard 4 | `"Temperature + 1 is near zero ..."` |
| `Temperature = -1.0000005` (near -1) | Guard 4 | `"Temperature + 1 is near zero ..."` |
| `Temperature = -0.999999` (denominator = 1e-6, at boundary) | Guard 4 | `"Temperature + 1 is near zero ..."` — skipped because guard uses `<=` |
| `Temperature = -0.99` (safe negative) | passes all guards | processed normally |

### What did NOT change

- Method signature — unchanged
- Return type — unchanged
- Business logic for valid readings — unchanged
- All classes remain in one file

---

## HISTORY

> Copilot: Do not use this section as implementation instructions.
> This section is for historical prompt documentation only.

<!--
## Session Timeline

### Step 1 — Task definition
User asked to analyze `EnergyAnalyticsService.cs`: explain what it does with examples,
then produce a full refactoring with a checklist. Analysis and checklist (10 items) were written in Parts 1 and 2 of this plan.

### Step 2 — Design questions before implementation
Four questions were raised before writing any code:
  (Q1) What name should replace the magic number `0.85`?
  (Q2) Should model classes be split into separate files?
  (Q3) How should an invalid temperature (division by zero) be handled?
  (Q4) Should aggregation use LINQ GroupBy or foreach + Dictionary?

### Step 3 — User answers
  (A1) Use `EfficiencyCoefficient`. Business meaning of 0.85 was not provided.
  (A2) Keep all classes in one file.
  (A3) Negative temperatures are valid. Only skip a reading when `Temperature + 1 == 0`
       exactly. Skipped readings must be tracked and visible — not silently lost.
  (A4) Use foreach + Dictionary (imperative, more readable).

### Step 4 — First implementation
All 10 checklist items were implemented. The method return type was initially changed to
`EfficiencyResult` (a new wrapper class) in order to return both device results and
skipped readings from the same method call.

### Step 5 — API break reverted
User rejected the return type change: it was a breaking API change.
The method signature was reverted to `List<DeviceResult>`.
Skipped readings are now exposed via a `LastSkippedReadings` property on the service
instance, populated and reset after every call.

### Step 6 — Verification with fake data
9 edge cases were traced manually against the code (see Part 3):
  - Correct running average across 3 readings (old formula was wrong)
  - Temperature = -1 skipped correctly
  - Negative temperature (not -1) processed normally
  - Empty input, null input, mixed batch, all-skipped batch, zero power, multiple calls
All 9 cases passed.

### Step 7 — Reflection review (5 concerns)
User asked for an explicit agree/disagree analysis on 5 concerns before any further changes:
  (Point 2) Null items inside the list are not handled — would crash.
  (Point 3) Null/empty/whitespace DeviceId is not validated — null would crash Dictionary.
  (Point 4) NaN or Infinity in numeric fields would silently corrupt accumulators.
  (Point 5) Exact `== 0` denominator check should be a near-zero epsilon guard.
  (Point 6) Scope should stay limited to correctness, performance, and safe input handling.
All 5 points were agreed with. An open question remained: what epsilon value to use for Point 5.

### Step 8 — Epsilon decision
User was asked to choose an epsilon for the near-zero denominator guard (4 options presented).
User chose Option 2: `1e-6`, named `MinDenominatorThreshold`.

### Step 9 — Second implementation (reflection fixes)
4 new guards added in the per-reading loop, in this order:
  (Guard 1) `item == null` → skip
  (Guard 2) `string.IsNullOrWhiteSpace(item.DeviceId)` → skip
  (Guard 3) `!double.IsFinite(Voltage/Current/Temperature)` → skip
  (Guard 4) `Math.Abs(Temperature + 1) < 1e-6` → skip
Checklist updated with items 11–14. Part 4 added with full guard documentation.
Method signature and return type remain unchanged.

### Step 10 — Second opinion review (Plan 002)
Plan 002 reviewed the refactored code against the original. 12 changes were confirmed correct.
One critical issue found: `EfficiencySum` had a public setter — internal accumulator was exposed
as mutable, allowing callers to silently corrupt `AverageEfficiency`.
One warning: `LastSkippedReadings` is instance state, not thread-safe for singleton DI usage.
One doc error: Plan 001 Part 4 table described T=-0.999999 as "skipped" — was wrong under `<` guard.

### Step 11 — Fixes from Plan 002 (user approved: STEP APPROVED)
3 fixes applied:
  (1) `EfficiencySum` → `{ get; private set; }`. Constructor and `AccumulateReading` method
      added to `DeviceResult`. Service uses these instead of direct object initializer.
  (2) Denominator guard changed from `<` to `<=`. T=-0.999999 is now correctly skipped.
      Plan 001 Part 4 guard line and table row updated accordingly.
  (3) XML `<remarks>` added to `EnergyAnalyticsService` warning against singleton DI registration.
-->
