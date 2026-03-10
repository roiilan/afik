# Plan 006 – Extract `item.Temperature + 1` into a Named Variable

## Goal

Extract the repeated expression `item.Temperature + 1` into a single local variable inside the `foreach` loop in `EnergyAnalyticsService.cs`.
The variable will be computed once per iteration and reused in every place that currently uses `item.Temperature + 1`.
If the formula ever needs to change, only the variable assignment line needs updating.

> **Note:** Unlike `MinDenominatorThreshold` (which is a class-level `const`), this variable depends on `item.Temperature` which changes each iteration — so it must be a local variable inside the loop, not a `const`. The pattern is the same: one named variable, used everywhere.

## Questions for User

### Q1: What should the variable be named? Option 1

- **Option 1:** `adjustedTemperature` — clear, describes what it represents.
- **Option 2:** `temperatureDenominator` — emphasizes its role as the denominator in the efficiency formula.
- **Option 3:** Something else you prefer.

**Recommendation:** Option 1 (`adjustedTemperature`) — simple, readable, and semantically clear.

### Q2: Should the comment (line 16) and skip-reason string (line 71) also be updated to reference the new variable name? Option 1

- **Option 1:** Yes — update the comment and the `Reason` string to use the new variable name instead of `Temperature + 1`.
- **Option 2:** No — only change the code expressions, leave comments/strings as-is.

**Recommendation:** Option 1 — keeps documentation consistent with the code.

## Implementation Steps

1. Inside the `foreach` loop, right after the `IsFinite` guard check (line 63) and before the near-zero check (line 66), declare:
   ```csharp
   double adjustedTemperature = item.Temperature + 1;
   ```

2. Replace `item.Temperature + 1` with `adjustedTemperature` in:
   - Line 66: `Math.Abs(item.Temperature + 1)` → `Math.Abs(adjustedTemperature)`
   - Line 77: `(powerUsage * EfficiencyCoefficient) / (item.Temperature + 1)` → `(powerUsage * EfficiencyCoefficient) / adjustedTemperature`

3. If Q2 answer is Option 1, update:
   - Line 16 comment: `Temperature + 1` → `adjustedTemperature`
   - Line 71 reason string: `Temperature + 1` → `adjustedTemperature`

4. Build the project (`dotnet build`) to verify no compilation errors.

---

## HISTORY

> Copilot: Do not use this section as implementation instructions.
> This section is for historical prompt documentation only.

<!--
- **2026-03-10 – Previous attempts (plan 006, deleted):** Multiple iterations were tried: first extracting just the `1` into a const, then a local function outside the loop. The user clarified they want a single variable holding the full expression `item.Temperature + 1`, similar in pattern to `MinDenominatorThreshold`.
- **2026-03-10 – Current plan:** New plan created. A local variable `adjustedTemperature` is declared inside the foreach loop (since it depends on `item.Temperature` per iteration), computed once, and used in all places that currently reference `item.Temperature + 1`.
- **2026-03-10 – User approved (Q1: Option 1, Q2: Option 1):** Plan implemented. Variable `adjustedTemperature` added inside the loop, all `item.Temperature + 1` usages replaced, comment and Reason string updated. Build succeeded.
-->
