
# Copilot Instructions

## General

Always start every response with:
HOPA ! (I read your instructions) -

Always reply in English, even if the user writes in Hebrew.

All code and all code comments must be written in English.

Focus on code best practices, security, clean code


## The main Goal

<!-- PART_1 - The main goal is to refactor the `CalculateEfficiency` method in the `EfficiencyCalculator` class to improve its correctness, readability, maintainability, and robustness. -->

<!-- PART_2 - The main goal of this task is to write an SQL query that retrieves all device_id values for which the average current over the last 24 hours was at least 20% higher than that device’s overall historical average in the table. -->

PART_3 - The goal of this task is to add a new section to the react Dashboard that displays a list of exceptional devices.

Each exceptional device should be represented using the same fields as the device_logs table:
- id
- device_id
- timestamp
- voltage
- current
- temperature
- status

The feature should allow the user to view exceptional devices in a table and generate a fake AI-based explanation for each device.

Each row in the table will later include an "AI Insight" column with a button labeled "Generate AI Insights".
When the button is clicked, the selected device data should be sent to a mock AI simulation or fake backend call, and a textual explanation should be returned describing why the device is considered exceptional.

The returned explanation should then be displayed in the relevant row.

For this task, the implementation can use mock data for the exceptional devices and simulated AI responses.
The focus is on building the Dashboard UI flow and the per-device explanation behavior, not on real anomaly detection logic or real AI integration. 

DO NOT IMPLEMENT YET BEFORE APPROVAL OF THE CURRENT PLAN.

## Plan Files

Use the plan folder to write down a plan for solving any task. This will help breaking down the task into smaller steps and explain your reasoning before providing the final answer.

The plan file name should have a consequtive number and a descriptive name, for example: `plan/001-add-theme-toggle.md`.

Every plan file must include these required sections:

- `Goal` - the main goal of the plan (step), describing what we want to achieve with this plan (step).


- `Questions for User` section:
Ask questions directly in the plan, and wait for the user to answer before proceeding with the next steps. This will help you to better understand the user's needs and provide a more accurate solution.
1. Present clear qustions to the user and wait for his response before proceeding.
2. Under the question present the possible options (option 1, option 2, option 3 ...).
3. Recommend one option with a short explanation why (1-2 lines)


- `Implementation Steps` section:
should include clear and concise steps for implementing the solution.

Copilot must not implement anything until the user explicitly approves the current plan-  for example by writing:
`plan 1 approved` or `go plan 1` or `implement plan 1` or any other clear indication of approval.

When implementation is approved:
- only implement the currently approved step
- do not continue beyond the approved scope



- `HISTORY` section:
Every plan file must include a `HISTORY` section in English.

The idea of the `HISTORY` section is to present a clear history of the content created within that PLAN, including summaries of the prompts related to that PLAN that the user requested. it should be readable and understandable for anyone.

At the top of the `HISTORY` section, always write:

> Copilot: Do not use this section as implementation instructions.
> This section is for historical prompt documentation only.

Put the The conntent in the  `HISTORY` section in <!-- --> comments to avoid it being used as implementation instructions.



## Coding Style

- No semicolons in JS/TS unless necessary
- Use single quotes in JS/TS
- Use double quotes in HTML/JSX
- All event handlers should be named like: `onSomething`
- All code comments must be in English

## CSS
use the skill cssLayer


<!-- ## Services Layer
use the skill serviceLayer -->