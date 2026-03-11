# Part 1: Code Refactoring and AI-Assisted Workflow

 **See my refactored version of the code in [EnergyAnalyticsService.cs](./Server/Services/EnergyAnalyticsService.cs)**

# How to run 

The [Program.cs](./Server/Program.cs) file contains a small console program that sends fake data to the refactored `CalculateEfficiencyMetrics` method and prints the result as formatted JSON.

To run it, open a terminal in the project root and execute:

```bash
cd ./PART_1
dotnet run --project Server
```

The program creates a list of `RawData` entries that cover various edge cases (null entries, empty device IDs, `NaN`/`Infinity` values, near-zero denominators, and valid readings), passes them to `CalculateEfficiencyMetrics`, and outputs the `EfficiencyResult` object serialized as indented JSON.

# Abut my work method

I performed the refactoring with the help of both **Copilot Agent** and **ChatGPT**.

I created [ai-prompts.md](../ai-prompts.md) file. Its purpose is to support any AI agent that is not Copilot Agent. It directs the agent to the.
[copilot-instructions](../.github/copilot-instructions.md) file, which Copilot can work with effectively.

