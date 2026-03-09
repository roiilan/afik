# Plan 003 - Wrapper Return Type for EnergyAnalyticsService

## Goal

Make `CalculateEfficiencyMetrics` return a single wrapper object containing both `List<DeviceResult>` and `List<SkippedReading>`, instead of storing `LastSkippedReadings` as mutable instance state on the service.

---

## Analysis: Did the Refactoring Change the Original Idea?

**No — the core idea is preserved.** The original and refactored versions both:
- Take `List<RawData>` as input
- Calculate `powerUsage = Voltage * Current`
- Calculate an efficiency factor using a `0.85` coefficient and temperature
- Aggregate results per device
- Return `List<DeviceResult>`

**What the refactoring fixed (bugs in the original):**
1. Redundant `Math.Pow(Math.Sqrt(x), 2)` → simplified to just `x`
2. Broken running average formula `(avg + new) / 2` → proper `EfficiencySum / ReadingsCount`

**What the refactoring added (not in original):**
1. Input validation (null items, empty DeviceId, NaN/Infinity, near-zero denominator)
2. `SkippedReading` tracking via `LastSkippedReadings` instance property
3. `Dictionary` for O(1) lookups instead of `FirstOrDefault` O(n²)
4. `AccumulateReading` method on `DeviceResult` for encapsulation

The method signature `List<DeviceResult> CalculateEfficiencyMetrics(List<RawData>)` was kept identical to avoid a breaking change. The `LastSkippedReadings` was added as a side-property — which is what this plan now proposes to clean up.

---

## Questions for User

1. **Wrapper class name?** option 1
   - Option 1: `EfficiencyResult` (short, descriptive) **(Recommended)**
   - Option 2: `EfficiencyMetricsResult`
   - Option 3: Other — please specify

2. **Should we remove `LastSkippedReadings` from the service after introducing the wrapper?** option 1
   - Option 1: Yes, remove it — the wrapper carries the data now **(Recommended)**
   - Option 2: Keep both (backward compatibility)

3. **Where to place the wrapper class?** option 1
   - Option 1: In the same file `EnergyAnalyticsService.cs` (consistent with current setup) **(Recommended)**
   - Option 2: Separate file

---

## Implementation Steps

Status: **COMPLETED**

1. ~~Create the `EfficiencyResult` wrapper class with `List<DeviceResult> DeviceResults` and `List<SkippedReading> SkippedReadings`~~ — Done
2. ~~Change the return type of `CalculateEfficiencyMetrics` from `List<DeviceResult>` to `EfficiencyResult`~~ — Done
3. ~~Update the method body to return the wrapper object~~ — Done
4. ~~Remove `LastSkippedReadings` property from the service~~ — Done
5. ~~Update XML doc comments~~ — Done (class-level and method-level docs updated; thread-safety warning removed since service is now stateless)

### What Changed

- **New class**: `EfficiencyResult` with `DeviceResults` and `SkippedReadings` properties
- **Return type**: `List<DeviceResult>` → `EfficiencyResult`
- **Removed**: `LastSkippedReadings` property from `EnergyAnalyticsService`
- **Updated docs**: Service is now stateless — safe for any DI lifetime (singleton, scoped, transient)


---

## HISTORY

> Copilot: Do not use this section as implementation instructions.
> This section is for historical prompt documentation only.

<!--
- Plan 001: Full refactoring of EnergyAnalyticsService — fixed bugs, added validation, added SkippedReading tracking, improved performance.
- Plan 002: Second-opinion review — found EfficiencySum public setter issue (fixed to private set), boundary condition fix (< to <=), thread-safety warning for singleton usage.
- Plan 003 (this): User asked whether the refactoring changed the original idea (answer: no, core logic preserved, bugs fixed, validation added). User requested a wrapper return type to bundle DeviceResult and SkippedReading together.
-->
