-- מבנה הטבלה לניתוח
CREATE TABLE device_logs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    device_id VARCHAR(50) NOT NULL,
    timestamp DATETIME NOT NULL,
    voltage DOUBLE,
    current DOUBLE,
    temperature DOUBLE,
    status VARCHAR(20)
);

-- הערה למועמד:
-- עליך לכתוב שאילתה השולפת חריגות (ממוצע 24 שעות מול ממוצע היסטורי)
-- ולהסביר את אסטרטגיית האינדוקס שלך עבור טבלה עם 10 מיליון רשומות.