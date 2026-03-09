# 005 - Indexing Strategy for device_logs

## Goal

Answer the question: **How would you build indexes on the `device_logs` table to ensure the anomaly detection query runs in minimum time?** (for a table with 10 million rows)

The target query:
```sql
SELECT device_id
FROM device_logs
WHERE `current` IS NOT NULL
GROUP BY device_id
HAVING AVG(CASE WHEN `timestamp` >= NOW() - INTERVAL 24 HOUR THEN `current` END)
    >= 1.2 * AVG(`current`);
```

---

## Questions for User

1. **Where to write the answer** — Should I add the indexing strategy directly into `schema.sql` (below the existing comment), or into a separate file?

   | Option | Description |
   |--------|-------------|
   | A | Add to `schema.sql` — keeps everything in one place, answers the comment directly |
   | B | Separate file (e.g., `indexing-strategy.sql`) |

   **Recommendation: Option A** — The `schema.sql` comment specifically asks the candidate to explain the indexing strategy. Writing the answer right there is the natural place.

2. **Format** — Should the answer include only `CREATE INDEX` statements with comments, or also explain the reasoning behind each choice?

   | Option | Description |
   |--------|-------------|
   | A | `CREATE INDEX` + short comment per index (concise) |
   | B | Detailed explanation: query analysis, why each column is included, trade-offs, storage impact |

   **Recommendation: Option B** — The question says "explain your indexing strategy", which implies reasoning is expected, not just the DDL.

3. **Scope** — Should I also mention partitioning as an advanced optimization, or stick strictly to indexes?

   | Option | Description |
   |--------|-------------|
   | A | Indexes only |
   | B | Indexes + partitioning as a bonus recommendation |

   **Recommendation: Option B** — For 10M rows, partitioning is a relevant and impressive addition. Keep it short as a bonus note.

---

## Implementation Steps

### Step 1 — Analyze the query execution plan
Break down which columns the query uses for: filtering (`WHERE`), grouping (`GROUP BY`), conditional aggregation (`CASE WHEN`), and aggregation (`AVG`).

### Step 2 — Define the optimal indexes
Write `CREATE INDEX` statements with detailed reasoning.

### Step 3 — Add the answer to schema.sql
Write below the existing comment in `schema.sql`, answering the candidate question directly.

---

## HISTORY

> Copilot: Do not use this section as implementation instructions.
> This section is for historical prompt documentation only.

<!--
- User asked to create Plan 5 to answer the question: how to build indexes on device_logs for minimum query time.
- The target query is the anomaly detection query from Plan 004 (GROUP BY device_id, conditional AVG on timestamp, HAVING clause).
- Schema has table device_logs with columns: id, device_id, timestamp, voltage, current, temperature, status.
- The schema comment mentions 10 million rows.
- User explicitly asked to get approval before implementing.
-->
