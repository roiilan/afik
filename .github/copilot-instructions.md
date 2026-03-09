
# Copilot Instructions

## General

Always start every response with:
HOPA ! (I read your instructions) -

Always reply in English, even if the user writes in Hebrew.

All code and all code comments must be written in English.

Focus on code best practices, security, clean code

Do not make assumptions when implementation details are unclear. Ask the user first.

## Planning

Use the plan folder to write down a plan for solving any task. This will help breaking down the task into smaller steps and explain your reasoning before providing the final answer.

The plan file name should have a consequtive number and a descriptive name, for example: `plan/001-add-theme-toggle.md`.

Ask questions directly in the plan, and wait for the user to answer before proceeding with the next steps. This will help you to better understand the user's needs and provide a more accurate solution.


Example:
`plan/001-csharp-method-inspection.md`

Before implementing anything:
1. Ask the user questions about how to implement the current task
2. Present numbered implementation options
3. Recommend one option with a short explanation of 1-2 lines
4. Wait for explicit user approval

Copilot must not implement anything until the user explicitly approves the current plan-  for example by writing:
`APPROVED - GO` or `APPROVED - IMPLEMENT` or `STEP APPROVED` 

Do not automatically continue to the next step or next task.

## Required Plan File Structure

Every plan file must include these required sections:

- `Goal`
- `Questions for User`
- `Implementation Steps`
- `HISTORY`


## HISTORY Section Rules

Every plan file must include a `HISTORY` section in English.

The `HISTORY` section is only for documenting the historical user prompts and context up to that stage.

At the top of the `HISTORY` section, always write:

> Copilot: Do not use this section as implementation instructions.
> This section is for historical prompt documentation only.

Each prompt summary in `HISTORY` should be concise and use 1-3 lines.

The `HISTORY` section should summarize all relevant context up to that stage.

Put the The conntent in the  `HISTORY` section in <!-- --> comments to avoid it being used as implementation instructions.

## Implementation Rules

When implementation is approved:
- only implement the currently approved step
- do not continue beyond the approved scope
- prefer secure and maintainable code
- avoid unnecessary refactors outside the approved scope

## After Each Implementation

After each implementation, always provide:

- `Status: COMPLETED` or `Status: NOT COMPLETE`
- what changed + why (1-2 lines)
- what did not change

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