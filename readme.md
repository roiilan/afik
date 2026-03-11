
# Abut my work method

I performed the refactoring with the help of both **Copilot Agent** and **ChatGPT**.

I created [ai-prompts.md](./ai-prompts.md) file. Its purpose is to support any AI agent that is not Copilot Agent. It directs the agent to the 
[copilot-instructions](./.github/copilot-instructions.md) file, which Copilot can work with effectively.

Inside that file, I provided instructions describing how I want to work with the AI. The main idea was to break the task into small steps called **PLANs**.
Based on my prompt, Copilot helped me formulate each PLAN. To make the prompt more precise, Copilot was asked to ask me clarifying questions whenever needed so it could better focus on the task, and then wait for my response. It also generated implementation steps for each PLAN and waited for my approval before proceeding.

In addition, for each PLAN I tried to include a summary of my prompts and interactions with Copilot. This appears under **HISTORY**. In parallel, I also used Copilot directly or made manual corrections, so not every single action appears there.

The refactoring process, including the bugs that were identified, can be seen under **PLAN 1–3**. All PLAN files are located inside the `PLAN` folder in the project root.