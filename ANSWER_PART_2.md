# Part 2: Data and Broader Thinking (Database - MySQL)

**see [schema.sql](./schema.sql)**

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
**see [schema.sql](./schema.sql)**

## Expected Result

```text
device_id |
+-----------+
| DEV-001   |
| DEV-002   |
| DEV-005   |
```
