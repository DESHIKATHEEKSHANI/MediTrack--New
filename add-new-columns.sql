-- Run this in MySQL Workbench to add the new columns to your existing Medications table

ALTER TABLE Medications
    ADD COLUMN MedicineType VARCHAR(30) NOT NULL DEFAULT 'Tablet',
    ADD COLUMN Frequency VARCHAR(50) NOT NULL DEFAULT 'Once daily',
    ADD COLUMN ReminderTimes VARCHAR(200) NOT NULL DEFAULT '',
    ADD COLUMN MealTiming VARCHAR(30) NOT NULL DEFAULT 'Any Time',
    ADD COLUMN StartDate DATETIME NULL,
    ADD COLUMN EndDate DATETIME NULL,
    ADD COLUMN IsOngoing TINYINT(1) NOT NULL DEFAULT 1,
    ADD COLUMN IsArchived TINYINT(1) NOT NULL DEFAULT 0,
    ADD COLUMN RemainingPills INT NULL,
    ADD COLUMN LowStockAlertAt INT NULL,
    ADD COLUMN SnoozeMinutes INT NULL;
