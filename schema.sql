
CREATE TABLE device_logs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    device_id VARCHAR(50) NOT NULL,
    timestamp DATETIME NOT NULL,
    voltage DOUBLE,
    current DOUBLE,
    temperature DOUBLE,
    status VARCHAR(20)
);

CREATE INDEX idx_device_timestamp_current
ON device_logs (device_id, `timestamp`, `current`);

CREATE INDEX idx_timestamp_device_current
ON device_logs (`timestamp`, device_id, `current`);

-- =====================
--  FAKE DATA
-- =====================

-- Clean slate (optional — remove if running against existing data)
DELETE FROM device_logs;

-- 6 devices:
--   DEV-001: POSITIVE  — last 24h avg current ~15.0, historical avg ~10.0 (50% above)
--   DEV-002: POSITIVE  — last 24h avg current ~12.0, historical avg ~8.0  (50% above)
--   DEV-003: NEGATIVE  — last 24h avg current ~5.0,  historical avg ~5.0  (0% above)
--   DEV-004: NEGATIVE  — last 24h avg current ~4.5,  historical avg ~5.0  (below)
--   DEV-005: BORDERLINE — last 24h avg current ~6.7, overall avg ~5.57 (~20.3%)
--   DEV-006: NEGATIVE  — last 24h avg current ~5.5,  historical avg ~5.0  (10%, under threshold)

INSERT INTO device_logs (device_id, `timestamp`, voltage, `current`, temperature, status) VALUES
-- -------------------------------------------------------
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




