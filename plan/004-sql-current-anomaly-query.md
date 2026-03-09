# 004 - SQL Current Anomaly Query

## Goal

Write a SQL query against the `device_logs` table (main root/ schema.sql) that returns all `device_id` values where the **average current in the last 24 hours** is at least **20% higher** than the **overall historical average current** for that same device. Generate fake data if needed to test the query.

---

## Questions for User

1. **Database engine** — The schema uses `AUTO_INCREMENT`, which is MySQL syntax. Should I target **MySQL 8+**, or do you need compatibility with another engine (PostgreSQL, SQL Server, etc.)?
   - **Answer:** MySQL ✅

2. **"Last 24 hours" reference point** — Should the query use `NOW()` as the reference, or a parameterized timestamp (e.g., for repeatable testing)? Option A

   | Option | Description |
   |--------|-------------|
   | A | `NOW()` — dynamic, always reflects the real last 24 hours |
   | B | A parameterized variable (e.g., `@ref_time`) — repeatable and testable |
   | C | Both — use a variable that defaults to `NOW()` |

   **Recommendation: Option C** — Use `SET @ref_time = NOW();` at the top. This gives production-ready behavior by default while allowing you to override the value for testing / reproducibility. Best of both worlds with zero cost.

3. **Fake data volume** — How much test data? option B

   | Option | Description |
   |--------|-------------|
   | A | Small (~20-30 rows, 3-4 devices) — quick to read and verify manually |
   | B | Medium (~100-200 rows, 5-6 devices) — more realistic distribution |
   | C | Large (1000+ rows via a generation script) — stress testing |

   **Recommendation: Option A** — For a correctness-focused exercise, a small set with clear positive / negative / borderline cases is the easiest to verify by hand. You can always scale up later.

4. **NULL handling** — What to do with rows where `current IS NULL`? Option A

   | Option | Description |
   |--------|-------------|
   | A | Exclude NULLs (`AVG` ignores them by default in MySQL) |
   | B | Treat NULLs as 0 (`COALESCE(current, 0)`) |

   **Recommendation: Option A** — `AVG()` in MySQL already ignores NULLs, which is the standard and most correct behavior. A NULL current means "no reading", not "zero amps". Treating it as 0 would artificially lower the average.

5. **Output columns** — What should the query return? OPTION A

   | Option | Description |
   |--------|-------------|
   | A | Only `device_id` |
   | B | `device_id` + `avg_last_24h` + `avg_historical` + `ratio` |

   **Recommendation: Option B** — Returning the extra columns costs nothing in performance and makes the results immediately verifiable and debuggable. In a real system you'd wrap this in a view or CTE and select only `device_id` if needed.

---

## Implementation Steps

### Step 1 — Generate fake data INSERT statements
Create `INSERT INTO device_logs` statements with:
- 3-4 distinct `device_id` values
- Historical records spread over several days
- At least 2 device whose last-24h average current is ≥ 20% above its historical average (positive case)
- At least one device that does NOT exceed the threshold (negative case)
- At least one borderline device (exactly ~20%)

### Step 2 — Write the anomaly detection query
Write a single SQL query that:
1. Computes each device's **overall historical average** current.
2. Computes each device's **last 24 hours average** current.
3. Filters to devices where `avg_24h >= 1.2 * avg_historical`.

### Step 3 — Explain indexing strategy
Add a short note on recommended indexes for a 10M-row table (as the schema comment requests).

### Step 4 — Deliver the final `.sql` file
Create a single SQL file containing:
- Fake data inserts
- The anomaly query
- Index recommendations as comments

---

## HISTORY

> Copilot: Do not use this section as implementation instructions.
> This section is for historical prompt documentation only.

<!--
- User asked to read schema.sql and write a SQL query that finds device_ids where the average current in the last 24 hours is at least 20% higher than the device's overall historical average.
- User asked to generate fake data if needed and to prepare a detailed work plan with steps.
- Schema contains a single table `device_logs` with columns: id, device_id, timestamp, voltage, current, temperature, status.
- User answered questions: MySQL, NOW(), medium data (~100-200 rows), exclude NULLs, output only device_id, at least 2 positive cases.
- Plan approved. Implementation delivered in anomaly-query.sql.
- User ran the query on MySQL. Got DEV-001 and DEV-002 but NOT DEV-005. Root cause: the 24h rows are included in AVG(current) (overall), raising DEV-005's historical avg from 5.0 to 5.33, making the ratio 1.125 instead of 1.20.
- Fixed: bumped DEV-005 last 24h current from 6.0 to 6.7 so overall avg ~5.57, ratio ~1.203 (borderline positive).
- User had trouble starting MySQL (service not installed, not running as admin). Guided through manual install steps.
- User asked to add this history and create Plan 5 to verify the task was completed successfully with tests. Note: anomaly-query.sql was deleted by user and needs to be recreated.
-->

## Status: COMPLETED



