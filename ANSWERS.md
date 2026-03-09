# Part 1: Code Refactoring and AI-Assisted Workflow

In the `Server/Services` folder, there are two files:

- [OriginalEnergyAnalyticsService.cs](./Server/Services/OriginalEnergyAnalyticsService.cs) — the original code I received.
-  [EnergyAnalyticsService.cs](./Server/Services/EnergyAnalyticsService.cs) — my refactored version of the code.

I performed the refactoring with the help of both **Copilot Agent** and **ChatGPT**.

I created [ai-prompts.md](./ai-prompts.md) file. Its purpose is to support any AI agent that is not Copilot Agent. It directs the agent to the 
[copilot-instructions](./.github/copilot-instructions.md) file, which Copilot can work with effectively.

Inside that file, I provided instructions describing how I want to work with the AI. The main idea was to break the task into small steps called **PLANs**.
Based on my prompt, Copilot helped me formulate each PLAN. To make the prompt more precise, Copilot was asked to ask me clarifying questions whenever needed so it could better focus on the task, and then wait for my response. It also generated implementation steps for each PLAN and waited for my approval before proceeding.

In addition, for each PLAN I tried to include a summary of my prompts and interactions with Copilot. This appears under **HISTORY**. In parallel, I also used Copilot directly or made manual corrections, so not every single action appears there.

The refactoring process, including the bugs that were identified, can be seen under **PLAN 1–3**. All PLAN files are located inside the `PLAN` folder in the project root.

-------------------------------------------------------------------------------

# Part 2: Data and Broader Thinking (Database - MySQL)

## Table Structure for Analysis

```sql
CREATE TABLE device_logs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    device_id VARCHAR(50) NOT NULL,
    timestamp DATETIME NOT NULL,
    voltage DOUBLE,
    current DOUBLE,
    temperature DOUBLE,
    status VARCHAR(20)
);
```

## Task

Given the `device_logs` table containing millions of rows:

**Write an SQL query that returns all `device_id` values where the average current in the last 24 hours was at least 20% higher than the overall historical average of the same device in the table.**

## Answer

```sql
SELECT h.device_id
FROM (
    SELECT device_id, AVG(`current`) AS historical_avg
    FROM device_logs
    WHERE `current` IS NOT NULL
    GROUP BY device_id
) h
JOIN (
    SELECT device_id, AVG(`current`) AS last_24h_avg
    FROM device_logs
    WHERE `current` IS NOT NULL
      AND `timestamp` >= NOW() - INTERVAL 24 HOUR
    GROUP BY device_id
) r ON r.device_id = h.device_id
WHERE r.last_24h_avg >= 1.2 * h.historical_avg;
```

## Analysis Question

**How would you build indexes on this table to ensure the query runs in minimum time?**

## Answer

To improve the execution time of the query on the `device_logs` table, I would create composite indexes that support both grouping by `device_id` and filtering by the last 24 hours.

The first index I would create is:

```sql
CREATE INDEX idx_device_timestamp_current
ON device_logs (device_id, `timestamp`, `current`);
```

This index is useful because the query performs a `GROUP BY` on `device_id`, uses `timestamp` to calculate the average current over the last 24 hours, and reads the `current` column to compute the averages. As a result, MySQL can access the required data more efficiently and, in some cases, may even be able to satisfy the query directly from the index without reading the full table rows.

In addition, since the query includes a time-based condition such as:

```sql
timestamp >= NOW() - INTERVAL 24 HOUR
```

I would also consider adding a second index:

```sql
CREATE INDEX idx_timestamp_device_current
ON device_logs (`timestamp`, device_id, `current`);
```

This index is especially helpful when MySQL chooses to first filter the rows from the last 24 hours and then group them by `device_id`.

However, it is important to note that even with good indexes, the query still needs to calculate the full historical average for each device across all rows in the table. Therefore, indexes alone will not completely eliminate wide scans of the data. If this query runs frequently in production on a very large table, the most efficient solution would be to maintain a summary table with pre-aggregated statistics for each `device_id`, instead of recalculating them from all 10 million rows every time.

In conclusion, I would recommend these two indexes:

```sql
CREATE INDEX idx_device_timestamp_current
ON device_logs (device_id, `timestamp`, `current`);

CREATE INDEX idx_timestamp_device_current
ON device_logs (`timestamp`, device_id, `current`);
```

The first index better supports grouping by device, and the second better supports filtering by time range. Together, they provide a solid optimization strategy for this query, although for best performance at scale I would also consider using a pre-aggregated summary table.

## Fake Data

```sql
-- DEV-001: POSITIVE CASE (historical avg ~10, last 24h ~15)
-- Historical data (older than 24h)
-- -------------------------------------------------------
('DEV-001', NOW() - INTERVAL 30 DAY, 220.0, 9.0, 35.0, 'active'),
('DEV-001', NOW() - INTERVAL 29 DAY, 221.0, 10.0, 36.0, 'active'),
('DEV-001', NOW() - INTERVAL 28 DAY, 219.0, 11.0, 34.0, 'active'),
('DEV-001', NOW() - INTERVAL 27 DAY, 220.0, 10.0, 35.0, 'active'),
('DEV-001', NOW() - INTERVAL 26 DAY, 222.0, 9.5, 36.0, 'active'),
('DEV-001', NOW() - INTERVAL 25 DAY, 218.0, 10.5, 33.0, 'active'),
('DEV-001', NOW() - INTERVAL 24 DAY, 220.0, 10.0, 35.0, 'active'),
('DEV-001', NOW() - INTERVAL 23 DAY, 221.0, 9.0, 34.0, 'active'),
('DEV-001', NOW() - INTERVAL 22 DAY, 219.0, 11.0, 36.0, 'active'),
('DEV-001', NOW() - INTERVAL 21 DAY, 220.0, 10.0, 35.0, 'active'),
('DEV-001', NOW() - INTERVAL 20 DAY, 220.0, 10.5, 35.0, 'active'),
('DEV-001', NOW() - INTERVAL 19 DAY, 221.0, 9.5, 36.0, 'active'),
('DEV-001', NOW() - INTERVAL 18 DAY, 219.0, 10.0, 34.0, 'active'),
('DEV-001', NOW() - INTERVAL 17 DAY, 220.0, 10.0, 35.0, 'active'),
('DEV-001', NOW() - INTERVAL 16 DAY, 222.0, 9.0, 36.0, 'active'),
('DEV-001', NOW() - INTERVAL 15 DAY, 218.0, 11.0, 33.0, 'active'),
('DEV-001', NOW() - INTERVAL 14 DAY, 220.0, 10.0, 35.0, 'active'),
('DEV-001', NOW() - INTERVAL 13 DAY, 221.0, 10.0, 34.0, 'active'),
('DEV-001', NOW() - INTERVAL 12 DAY, 219.0, 10.0, 36.0, 'active'),
('DEV-001', NOW() - INTERVAL 11 DAY, 220.0, 10.0, 35.0, 'active'),
-- DEV-001: Last 24h data (high current ~15)
('DEV-001', NOW() - INTERVAL 20 HOUR, 220.0, 14.0, 40.0, 'active'),
('DEV-001', NOW() - INTERVAL 16 HOUR, 221.0, 15.0, 41.0, 'active'),
('DEV-001', NOW() - INTERVAL 12 HOUR, 219.0, 16.0, 42.0, 'active'),
('DEV-001', NOW() - INTERVAL 8 HOUR, 220.0, 15.0, 40.0, 'active'),
('DEV-001', NOW() - INTERVAL 4 HOUR, 222.0, 14.5, 39.0, 'active'),
('DEV-001', NOW() - INTERVAL 2 HOUR, 220.0, 15.5, 41.0, 'active'),

-- -------------------------------------------------------
-- DEV-002: POSITIVE CASE (historical avg ~8, last 24h ~12)
-- -------------------------------------------------------
('DEV-002', NOW() - INTERVAL 30 DAY, 220.0, 7.0, 30.0, 'active'),
('DEV-002', NOW() - INTERVAL 28 DAY, 221.0, 8.0, 31.0, 'active'),
('DEV-002', NOW() - INTERVAL 26 DAY, 219.0, 9.0, 29.0, 'active'),
('DEV-002', NOW() - INTERVAL 24 DAY, 220.0, 8.0, 30.0, 'active'),
('DEV-002', NOW() - INTERVAL 22 DAY, 222.0, 7.5, 31.0, 'active'),
('DEV-002', NOW() - INTERVAL 20 DAY, 218.0, 8.5, 28.0, 'active'),
('DEV-002', NOW() - INTERVAL 18 DAY, 220.0, 8.0, 30.0, 'active'),
('DEV-002', NOW() - INTERVAL 16 DAY, 221.0, 7.0, 29.0, 'active'),
('DEV-002', NOW() - INTERVAL 14 DAY, 219.0, 9.0, 31.0, 'active'),
('DEV-002', NOW() - INTERVAL 12 DAY, 220.0, 8.0, 30.0, 'active'),
('DEV-002', NOW() - INTERVAL 10 DAY, 220.0, 7.5, 30.0, 'active'),
('DEV-002', NOW() - INTERVAL 8 DAY, 221.0, 8.5, 31.0, 'active'),
('DEV-002', NOW() - INTERVAL 6 DAY, 219.0, 8.0, 29.0, 'active'),
('DEV-002', NOW() - INTERVAL 4 DAY, 220.0, 8.0, 30.0, 'active'),
('DEV-002', NOW() - INTERVAL 3 DAY, 222.0, 7.0, 31.0, 'active'),
('DEV-002', NOW() - INTERVAL 2 DAY, 218.0, 9.0, 28.0, 'active'),
-- DEV-002: Last 24h data (high current ~12)
('DEV-002', NOW() - INTERVAL 22 HOUR, 220.0, 11.0, 35.0, 'active'),
('DEV-002', NOW() - INTERVAL 18 HOUR, 221.0, 12.0, 36.0, 'active'),
('DEV-002', NOW() - INTERVAL 14 HOUR, 219.0, 13.0, 37.0, 'active'),
('DEV-002', NOW() - INTERVAL 10 HOUR, 220.0, 12.0, 35.0, 'active'),
('DEV-002', NOW() - INTERVAL 6 HOUR, 222.0, 11.5, 34.0, 'active'),
('DEV-002', NOW() - INTERVAL 3 HOUR, 220.0, 12.5, 36.0, 'active'),

-- -------------------------------------------------------
-- DEV-003: NEGATIVE CASE (historical avg ~5, last 24h ~5 — no anomaly)
-- -------------------------------------------------------
('DEV-003', NOW() - INTERVAL 30 DAY, 220.0, 5.0, 25.0, 'active'),
('DEV-003', NOW() - INTERVAL 27 DAY, 221.0, 5.5, 26.0, 'active'),
('DEV-003', NOW() - INTERVAL 24 DAY, 219.0, 4.5, 24.0, 'active'),
('DEV-003', NOW() - INTERVAL 21 DAY, 220.0, 5.0, 25.0, 'active'),
('DEV-003', NOW() - INTERVAL 18 DAY, 222.0, 5.0, 26.0, 'active'),
('DEV-003', NOW() - INTERVAL 15 DAY, 218.0, 5.0, 23.0, 'active'),
('DEV-003', NOW() - INTERVAL 12 DAY, 220.0, 5.5, 25.0, 'active'),
('DEV-003', NOW() - INTERVAL 9 DAY, 221.0, 4.5, 24.0, 'active'),
('DEV-003', NOW() - INTERVAL 6 DAY, 219.0, 5.0, 26.0, 'active'),
('DEV-003', NOW() - INTERVAL 4 DAY, 220.0, 5.0, 25.0, 'active'),
('DEV-003', NOW() - INTERVAL 3 DAY, 220.0, 5.0, 25.0, 'active'),
('DEV-003', NOW() - INTERVAL 2 DAY, 221.0, 5.5, 26.0, 'active'),
-- DEV-003: Last 24h data (same ~5 avg)
('DEV-003', NOW() - INTERVAL 20 HOUR, 220.0, 5.0, 25.0, 'active'),
('DEV-003', NOW() - INTERVAL 16 HOUR, 221.0, 5.5, 26.0, 'active'),
('DEV-003', NOW() - INTERVAL 12 HOUR, 219.0, 4.5, 24.0, 'active'),
('DEV-003', NOW() - INTERVAL 8 HOUR, 220.0, 5.0, 25.0, 'active'),
('DEV-003', NOW() - INTERVAL 4 HOUR, 222.0, 5.0, 26.0, 'active'),
('DEV-003', NOW() - INTERVAL 1 HOUR, 220.0, 5.0, 25.0, 'active'),

-- -------------------------------------------------------
-- DEV-004: NEGATIVE CASE (historical avg ~5, last 24h ~4.5 — below)
-- -------------------------------------------------------
('DEV-004', NOW() - INTERVAL 30 DAY, 220.0, 5.0, 28.0, 'active'),
('DEV-004', NOW() - INTERVAL 27 DAY, 221.0, 5.5, 29.0, 'active'),
('DEV-004', NOW() - INTERVAL 24 DAY, 219.0, 4.5, 27.0, 'active'),
('DEV-004', NOW() - INTERVAL 21 DAY, 220.0, 5.0, 28.0, 'active'),
('DEV-004', NOW() - INTERVAL 18 DAY, 222.0, 5.0, 29.0, 'active'),
('DEV-004', NOW() - INTERVAL 15 DAY, 218.0, 5.5, 26.0, 'active'),
('DEV-004', NOW() - INTERVAL 12 DAY, 220.0, 5.0, 28.0, 'active'),
('DEV-004', NOW() - INTERVAL 9 DAY, 221.0, 4.5, 27.0, 'active'),
('DEV-004', NOW() - INTERVAL 6 DAY, 219.0, 5.0, 29.0, 'active'),
('DEV-004', NOW() - INTERVAL 4 DAY, 220.0, 5.0, 28.0, 'active'),
('DEV-004', NOW() - INTERVAL 3 DAY, 220.0, 5.0, 28.0, 'active'),
('DEV-004', NOW() - INTERVAL 2 DAY, 221.0, 5.5, 29.0, 'active'),
-- DEV-004: Last 24h data (lower ~4.5 avg)
('DEV-004', NOW() - INTERVAL 20 HOUR, 220.0, 4.0, 27.0, 'active'),
('DEV-004', NOW() - INTERVAL 16 HOUR, 221.0, 4.5, 28.0, 'active'),
('DEV-004', NOW() - INTERVAL 12 HOUR, 219.0, 5.0, 26.0, 'active'),
('DEV-004', NOW() - INTERVAL 8 HOUR, 220.0, 4.5, 27.0, 'active'),
('DEV-004', NOW() - INTERVAL 4 HOUR, 222.0, 4.0, 28.0, 'active'),
('DEV-004', NOW() - INTERVAL 1 HOUR, 220.0, 5.0, 27.0, 'active'),

-- -------------------------------------------------------
-- DEV-005: BORDERLINE CASE (historical avg ~5.0, last 24h ~6.0 — exactly 20%)
-- -------------------------------------------------------
('DEV-005', NOW() - INTERVAL 30 DAY, 220.0, 5.0, 22.0, 'active'),
('DEV-005', NOW() - INTERVAL 27 DAY, 221.0, 5.5, 23.0, 'active'),
('DEV-005', NOW() - INTERVAL 24 DAY, 219.0, 4.5, 21.0, 'active'),
('DEV-005', NOW() - INTERVAL 21 DAY, 220.0, 5.0, 22.0, 'active'),
('DEV-005', NOW() - INTERVAL 18 DAY, 222.0, 5.0, 23.0, 'active'),
('DEV-005', NOW() - INTERVAL 15 DAY, 218.0, 5.0, 20.0, 'active'),
('DEV-005', NOW() - INTERVAL 12 DAY, 220.0, 5.5, 22.0, 'active'),
('DEV-005', NOW() - INTERVAL 9 DAY, 221.0, 4.5, 21.0, 'active'),
('DEV-005', NOW() - INTERVAL 6 DAY, 219.0, 5.0, 23.0, 'active'),
('DEV-005', NOW() - INTERVAL 4 DAY, 220.0, 5.0, 22.0, 'active'),
('DEV-005', NOW() - INTERVAL 3 DAY, 220.0, 5.0, 22.0, 'active'),
('DEV-005', NOW() - INTERVAL 2 DAY, 221.0, 5.0, 23.0, 'active'),
-- DEV-005: Last 24h data (avg 6.7 => overall avg ~5.57 => 6.7/5.57 = 1.203 => ~20% borderline)
('DEV-005', NOW() - INTERVAL 20 HOUR, 220.0, 6.7, 24.0, 'active'),
('DEV-005', NOW() - INTERVAL 16 HOUR, 221.0, 6.7, 25.0, 'active'),
('DEV-005', NOW() - INTERVAL 12 HOUR, 219.0, 6.7, 23.0, 'active'),
('DEV-005', NOW() - INTERVAL 8 HOUR, 220.0, 6.7, 24.0, 'active'),
('DEV-005', NOW() - INTERVAL 4 HOUR, 222.0, 6.7, 25.0, 'active'),
('DEV-005', NOW() - INTERVAL 1 HOUR, 220.0, 6.7, 24.0, 'active'),

-- -------------------------------------------------------
-- DEV-006: NEGATIVE CASE (historical avg ~5.0, last 24h ~5.5 — only 10%)
-- -------------------------------------------------------
('DEV-006', NOW() - INTERVAL 30 DAY, 220.0, 5.0, 32.0, 'active'),
('DEV-006', NOW() - INTERVAL 27 DAY, 221.0, 5.5, 33.0, 'active'),
('DEV-006', NOW() - INTERVAL 24 DAY, 219.0, 4.5, 31.0, 'active'),
('DEV-006', NOW() - INTERVAL 21 DAY, 220.0, 5.0, 32.0, 'active'),
('DEV-006', NOW() - INTERVAL 18 DAY, 222.0, 5.0, 33.0, 'active'),
('DEV-006', NOW() - INTERVAL 15 DAY, 218.0, 5.0, 30.0, 'active'),
('DEV-006', NOW() - INTERVAL 12 DAY, 220.0, 5.5, 32.0, 'active'),
('DEV-006', NOW() - INTERVAL 9 DAY, 221.0, 4.5, 31.0, 'active'),
('DEV-006', NOW() - INTERVAL 6 DAY, 219.0, 5.0, 33.0, 'active'),
('DEV-006', NOW() - INTERVAL 4 DAY, 220.0, 5.0, 32.0, 'active'),
('DEV-006', NOW() - INTERVAL 3 DAY, 220.0, 5.0, 32.0, 'active'),
('DEV-006', NOW() - INTERVAL 2 DAY, 221.0, 5.5, 33.0, 'active'),
-- DEV-006: Last 24h data (~5.5 avg => 5.5 / 5.0 = 1.10 => only 10%)
('DEV-006', NOW() - INTERVAL 20 HOUR, 220.0, 5.5, 33.0, 'active'),
('DEV-006', NOW() - INTERVAL 16 HOUR, 221.0, 5.5, 34.0, 'active'),
('DEV-006', NOW() - INTERVAL 12 HOUR, 219.0, 5.5, 32.0, 'active'),
('DEV-006', NOW() - INTERVAL 8 HOUR, 220.0, 5.5, 33.0, 'active'),
('DEV-006', NOW() - INTERVAL 4 HOUR, 222.0, 5.5, 34.0, 'active'),
('DEV-006', NOW() - INTERVAL 1 HOUR, 220.0, 5.5, 33.0, 'active');
```

## Expected Result

```text
device_id |
+-----------+
| DEV-001   |
| DEV-002   |
| DEV-005   |
```
