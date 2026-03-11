# Part 2: Data and Broader Thinking (Database - MySQL)

**See the solution in [schema.sql](./schema.sql)**

## Task

Given the `device_logs` table containing millions of rows.

Write an SQL query that returns all `device_id` values where the average `current` in the last 24 hours was at least 20% higher than the overall historical average of the same device in the table.

## Answer

~~~sql
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
~~~

The query splits the problem into two separate calculations for each `device_id`:  
the overall historical average, and the average over the last 24 hours. It then joins the two result sets by `device_id` and filters only the devices where the average current in the last 24 hours is at least 20% higher than the historical average.

I added `current IS NOT NULL` to ensure the calculation is based only on valid values. Although `AVG()` ignores `NULL` values by default, filtering them explicitly makes the intent clearer and keeps the query clean.

## Analysis Question

How would you build indexes on this table to ensure the query runs in minimum time?

## Answer
To improve the execution time of the query on the `device_logs` table, I would create composite indexes that support both grouping by `device_id` and filtering by the last 24 hours.

~~~sql
CREATE INDEX idx_device_timestamp_current
ON device_logs (device_id, `timestamp`, `current`);

CREATE INDEX idx_timestamp_device_current
ON device_logs (`timestamp`, device_id, `current`);
~~~

However, I would not assume in advance that both indexes must always be kept without checking the actual execution plan with `EXPLAIN`, because each index also has a storage cost and adds overhead to `INSERT` and `UPDATE` operations.

### The first index:

~~~sql
CREATE INDEX idx_device_timestamp_current
ON device_logs (device_id, `timestamp`, `current`);
~~~

This index is useful because the query performs a `GROUP BY` on `device_id`, uses `timestamp` to filter the rows from the last 24 hours, and reads the `current` column to calculate the averages. In some cases, it may also allow MySQL to satisfy part of the query directly from the index without reading the full table rows.

### The second index:

~~~sql
CREATE INDEX idx_timestamp_device_current
ON device_logs (`timestamp`, device_id, `current`);
~~~

This index can be especially useful when MySQL chooses to first filter the rows from the last 24 hours and only then group them by `device_id`.
Since the query includes a time-based condition such as:

~~~sql
timestamp >= NOW() - INTERVAL 24 HOUR
~~~

this index may better support the time-range filtering part of the query.



## Run it online

You can run it online here:  
https://onecompiler.com/mysql/44fuz9fjg

## Expected Result

~~~text
device_id |
+-----------+
| DEV-001   |
| DEV-002   |
| DEV-005   |
~~~