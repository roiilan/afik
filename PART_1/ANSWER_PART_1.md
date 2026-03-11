# Part 1: Code Refactoring and AI-Assisted Workflow

**See my refactored version of the code in [EnergyAnalyticsService.cs](./Server/Services/EnergyAnalyticsService.cs).**

## How to Run

The [Program.cs](./Server/Program.cs) file contains a small console program that sends fake data to the refactored `CalculateEfficiencyMetrics` method and prints the result as formatted JSON.

To run it, open a terminal in the project root and execute:

```bash
cd ./PART_1
dotnet run --project Server
```

The program creates a list of `RawData` entries that covers several edge cases, including null entries, empty device IDs, `NaN` / `Infinity` values, near-zero denominators, and valid readings. It then passes the data to `CalculateEfficiencyMetrics` and prints the resulting `EfficiencyResult` object as indented JSON.

## Issues Found in the Original Code
 [OriginalEnergyAnalyticsService.cs](./Server/Services/OriginalEnergyAnalyticsService.cs)


The original implementation had several logic, performance, and maintainability problems:

- **Incorrect average calculation**  
  The original code updated `AverageEfficiency` using `(currentAverage + newValue) / 2`, which does not calculate the true average across all readings.

- **Unnecessary mathematical complexity**  
  The expression `Math.Pow(Math.Sqrt(powerUsage * 0.85), 2)` was unnecessarily complex and could be simplified without changing the effective result.

- **Inefficient lookup per reading**  
  The code searched the results list with `FirstOrDefault(...)` inside the loop, making the aggregation less efficient as the number of devices increased.

- **No input validation**  
  The original version did not handle `null` input, `null` items, missing or empty `DeviceId` values, or invalid numeric values such as `NaN` and `Infinity`.

- **Risk of division by zero or unstable values**  
  The denominator used `Temperature + 1` without checking whether it was zero or extremely close to zero.

- **Low readability and maintainability**  
  Important calculation values such as `0.85` were hardcoded, and the code did not clearly separate validation, calculation, and aggregation concerns.

## What I Changed

The refactored version improves the implementation in several ways:

- Replaced list-based lookup with a `Dictionary<string, DeviceResult>` for more efficient aggregation.
- Fixed the average calculation by accumulating `EfficiencySum` and deriving `AverageEfficiency` from it.
- Simplified the efficiency formula to make it easier to understand and maintain.
- Added validation for invalid input and unsafe readings.
- Introduced `SkippedReading` so invalid records are explicitly tracked instead of silently ignored or causing failures.
- Wrapped the output in an `EfficiencyResult` object to return both successful aggregated results and skipped readings together.
- Added comments and clearer naming to improve readability.

## Notes / Design Decisions

### 1. Method Signature Change

I chose to change the method signature so it returns EfficiencyResult instead of the original List `<DeviceResult>.`

This change was intentional. The refactored implementation not only aggregates valid device results, but also tracks invalid or unsafe readings that were skipped during processing. Returning `EfficiencyResult` makes it possible to expose both:
- `DeviceResults`
- `SkippedReadings`

in a single structured response.

I considered this a better design choice because it preserves useful diagnostic information that would otherwise be lost.

### 2. Use of `double.IsFinite(...)`

I chose to keep the current use of `double.IsFinite(...)` because it makes the validation logic concise and clear.

However, this API is not available in older .NET versions, so the code may require adjustment if the project targets an older framework. In such cases, it can be replaced with explicit checks using `double.IsNaN(...)` and `double.IsInfinity(...)`.

### 3. Business Rules Not Explicitly Required

I did not add extra business-rule validation for cases such as negative `Voltage`, negative `Current`, or negative adjusted temperature values beyond the current denominator safety check.

This was a deliberate decision because these rules were not explicitly part of the task requirements, and the current implementation still works correctly from a technical perspective.

That said, these cases should be considered in a real production system, depending on the business meaning of the sensor data and whether such values are valid in the domain.

## About My Work Method

I performed the refactoring with the help of both **Copilot Agent** and **ChatGPT**.

I created the [ai-prompts.md](../ai-prompts.md) file to support AI agents other than Copilot Agent. This file directs the agent to the [copilot-instructions](../.github/copilot-instructions.md) file, which Copilot can use effectively.

In these files, I described how I wanted the AI to assist me. The main idea was to break the work into small, manageable steps called **PLANs**.

Based on these instructions, Copilot Agent created the PLANs. For each PLAN, I reviewed the logic and the proposed implementation steps, and corrected them when necessary. I approved each PLAN only after I was confident it was correct, and the implementation was carried out only after my approval. I also answered Copilot’s clarifying questions when needed, in order to refine its understanding and improve the accuracy of the following steps.

Each PLAN also includes a **HISTORY** section summarizing the main prompts and interactions with Copilot Agent.

This review and approval process can be seen in **PLAN 1–3**, which are located in the `PLAN` folder in the project root.

After that, I used **ChatGPT** as an additional review layer. I asked it to refactor the original code and provide a list of issues that should be fixed. Then I asked it to review the solution I had already developed with Copilot Agent, give its opinion, and verify whether the important changes had been addressed.

I read ChatGPT’s comments carefully, asked questions where something was unclear, and chose to focus mainly on the issues I considered material.

For reference, here is a shared link to the ChatGPT conversation used during this process: **=(https://chatgpt.com/share/69b167fc-3f74-8011-81fe-c111ec780002)**