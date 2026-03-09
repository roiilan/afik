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

- [x] **4. Division by zero risk**: Skip readings where `Temperature + 1 == 0`. Skipped readings are exposed via `IReadOnlyList<SkippedReading> LastSkippedReadings` property on the service (non-breaking - return type unchanged). `LastSkippedReadings` is reset on every call, so stale data from a previous call never leaks through.

### Performance Issues

- [x] **5. O(n×m) lookup with `FirstOrDefault`**: Replaced with `Dictionary<string, DeviceResult>` for O(1) lookups.

### Clean Code & Best Practices

- [x] **6. Input validation**: Added `ArgumentNullException` guard clause for null `data` parameter.

- [x] **7. Nullable reference types**: All string properties now default to `string.Empty`.

- [ ] **8. Separation of concerns**: User chose to keep all classes in one file (Option 1). No change needed.

- [x] **9. Naming**: Internal variables are clear and descriptive. No change needed.

- [x] **10. Immutability**: `AverageEfficiency` is now a computed read-only property. `EfficiencySum` is internal for accumulation.

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

## HISTORY

> Copilot: Do not use this section as implementation instructions.
> This section is for historical prompt documentation only.

<!--
- User requested to create plan1: analyze EnergyAnalyticsService.cs, explain what it does with examples, then perform a full refactoring.
- User asked for a checklist of refactoring items.
- User answered all 4 questions: use EfficiencyCoefficient, keep all in one file, skip readings where Temperature+1==0 and track them, use foreach+Dictionary.
- User approved implementation: "implement optimal refactoring".
- All 10 checklist items implemented (8 done, 1 skipped per user choice, 1 no change needed).
- User requested: revert method signature to List<DeviceResult> (no breaking change). Track skipped readings via service property LastSkippedReadings instead.
- User requested: verify with fake data + edge cases and document in plan. 9 cases traced manually — all pass.
-->
