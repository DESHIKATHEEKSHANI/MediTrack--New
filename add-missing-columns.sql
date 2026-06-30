-- Run each line one by one in MySQL Workbench.
-- If a line throws "Duplicate column name", skip it — that column already exists.
-- Run the rest until all missing columns are added.

ALTER TABLE Medications ADD COLUMN MedicineType VARCHAR(30) NOT NULL DEFAULT 'Tablet';
ALTER TABLE Medications ADD COLUMN Frequency VARCHAR(50) NOT NULL DEFAULT 'Once daily';
ALTER TABLE Medications ADD COLUMN ReminderTimes VARCHAR(200) NOT NULL DEFAULT '';
ALTER TABLE Medications ADD COLUMN MealTiming VARCHAR(30) NOT NULL DEFAULT 'Any Time';
ALTER TABLE Medications ADD COLUMN StartDate DATETIME NULL;
ALTER TABLE Medications ADD COLUMN EndDate DATETIME NULL;
ALTER TABLE Medications ADD COLUMN IsOngoing TINYINT(1) NOT NULL DEFAULT 1;
ALTER TABLE Medications ADD COLUMN IsArchived TINYINT(1) NOT NULL DEFAULT 0;
ALTER TABLE Medications ADD COLUMN RemainingPills INT NULL;
ALTER TABLE Medications ADD COLUMN LowStockAlertAt INT NULL;
ALTER TABLE Medications ADD COLUMN SnoozeMinutes INT NULL;
