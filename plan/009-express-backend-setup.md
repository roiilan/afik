# Plan 009 – Express Backend Setup

## Goal

Set up a minimal Express server inside `react_proj/backend` that will serve as the mock backend for AI insight generation in future steps. This plan covers only the server foundation — no AI logic or device rules.

---

## Questions for User

### Q1: Should we add a `start` script using `nodemon` for auto-reload during development, or just a plain `node server.js` script? Option 2

- **Option 1:** Plain `node server.js` — minimal, no extra dependency.
- **Option 2:** Add `nodemon` as a dev dependency with a `dev` script for auto-reload.

**Recommendation:** Option 2 — `nodemon` makes development smoother with auto-restart on file changes, and it's only a dev dependency so it doesn't affect production.

### Q2: Should we use ES Modules (`import/export`) or CommonJS (`require/module.exports`) syntax? Option 2

- **Option 1:** CommonJS (`require`) — classic Node.js style, zero config needed.
- **Option 2:** ES Modules (`import`) — modern syntax, requires `"type": "module"` in `package.json`.

**Recommendation:** Option 2 (ES Modules) — aligns with the frontend's modern JS style and keeps the codebase consistent.

---

## Implementation Steps

1. **Initialize the backend project**
   - Run `npm init -y` inside `react_proj/backend`
   - Set `"type": "module"` in `package.json` (if ES Modules approved)
   - Add `start` and `dev` scripts

2. **Install dependencies**
   - `npm install express cors`
   - `npm install --save-dev nodemon` (if approved)

3. **Create `server.js`**
   - Import `express` and `cors`
   - Create an Express app
   - Enable JSON parsing (`express.json()`)
   - Enable CORS middleware
   - Add a health-check route: `GET /api/health` → `{ "status": "ok" }`
   - Listen on port `3001`

4. **Verify the server**
   - Start the server and confirm `GET /api/health` returns `{ "status": "ok" }`

---

## HISTORY

> Copilot: Do not use this section as implementation instructions.
> This section is for historical prompt documentation only.

<!--
- **Prompt 1 (2026-03-10):** User requested a new plan to set up a simple Express backend inside react_proj/backend. Requirements: express + cors, server.js as entry point, JSON parsing, CORS enabled, port 3001, GET /api/health route returning { "status": "ok" }. No AI insight logic or device rules — server foundation only.
- **Prompt 2 (2026-03-10):** User approved Plan 9 (chose Option 2 for both questions: nodemon + ES Modules). Implementation completed:
  - Initialized `react_proj/backend` with `npm init`, set `"type": "module"`, added `start` and `dev` scripts.
  - Installed `express`, `cors` (production) and `nodemon` (dev).
  - Created `server.js` with Express + CORS + JSON parsing + `GET /api/health` route on port 3001.
  - Verified server starts and health endpoint returns `{ "status": "ok" }`.
- **Prompt 3 (2026-03-10):** User asked for guidance on how to manually verify that everything is working correctly.
-->
