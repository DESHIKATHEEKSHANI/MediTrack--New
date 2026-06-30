-- Drop existing database and create new normalized schema
DROP DATABASE IF EXISTS `meditrack-db`;
CREATE DATABASE `meditrack-db` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `meditrack-db`;

-- Users table
CREATE TABLE users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(256) NOT NULL,
    FullName VARCHAR(100) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    LastLoginAt DATETIME NULL,
    IsActive TINYINT(1) NOT NULL DEFAULT 1
);

-- Medications: core info only
CREATE TABLE medications (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    OfficialName VARCHAR(150) NOT NULL,
    DisplayName VARCHAR(150) NULL,
    DosageValue DOUBLE NOT NULL,
    DosageUnit VARCHAR(20) NOT NULL,
    MedicineType VARCHAR(30) NOT NULL DEFAULT 'Tablet',
    IntakeInstructions VARCHAR(500) NOT NULL DEFAULT '',
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    IsArchived TINYINT(1) NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (UserId) REFERENCES users(Id) ON DELETE CASCADE
);

-- Medication schedules: when to take (one row per dose time)
CREATE TABLE medication_schedules (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    MedicationId INT NOT NULL,
    ReminderTime TIME NOT NULL,
    WeekdaySchedule JSON NOT NULL,
    MealTiming VARCHAR(30) NOT NULL DEFAULT 'Any Time',
    StartDate DATE NULL,
    EndDate DATE NULL,
    IsOngoing TINYINT(1) NOT NULL DEFAULT 1,
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    FOREIGN KEY (MedicationId) REFERENCES medications(Id) ON DELETE CASCADE
);

-- Medication inventory: stock tracking
CREATE TABLE medication_inventory (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    MedicationId INT NOT NULL UNIQUE,
    RemainingPills INT NULL,
    LowStockAlertAt INT NULL,
    FOREIGN KEY (MedicationId) REFERENCES medications(Id) ON DELETE CASCADE
);

-- Snooze settings per medication
CREATE TABLE medication_snooze (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    MedicationId INT NOT NULL UNIQUE,
    SnoozeMinutes INT NOT NULL DEFAULT 5,
    FOREIGN KEY (MedicationId) REFERENCES medications(Id) ON DELETE CASCADE
);

-- Reminders: daily generated instances (Pending, Taken, Missed, Snoozed)
CREATE TABLE reminders (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    MedicationId INT NOT NULL,
    ScheduleId INT NOT NULL,
    ScheduledDateTime DATETIME NOT NULL,
    Status INT NOT NULL DEFAULT 1 COMMENT '0=Taken,1=Pending,2=Missed,3=Snoozed',
    SnoozeUntil DATETIME NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (MedicationId) REFERENCES medications(Id) ON DELETE CASCADE,
    FOREIGN KEY (ScheduleId) REFERENCES medication_schedules(Id) ON DELETE CASCADE
);

-- Intake logs: pure audit trail when user acts on a reminder
CREATE TABLE intake_logs (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ReminderId INT NOT NULL,
    UserId INT NOT NULL,
    MedicationId INT NOT NULL,
    ActionTimestamp DATETIME NULL,
    Status INT NOT NULL COMMENT '0=Taken,2=Missed',
    LoggedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ReminderId) REFERENCES reminders(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES users(Id) ON DELETE CASCADE,
    FOREIGN KEY (MedicationId) REFERENCES medications(Id) ON DELETE CASCADE
);

-- Indexes for performance
CREATE INDEX idx_medications_user ON medications(UserId);
CREATE INDEX idx_schedules_med ON medication_schedules(MedicationId);
CREATE INDEX idx_reminders_med ON reminders(MedicationId);
CREATE INDEX idx_reminders_schedule ON reminders(ScheduleId);
CREATE INDEX idx_reminders_status ON reminders(Status);
CREATE INDEX idx_reminders_datetime ON reminders(ScheduledDateTime);
CREATE INDEX idx_intakelogs_user ON intake_logs(UserId);
CREATE INDEX idx_intakelogs_med ON intake_logs(MedicationId);
CREATE INDEX idx_intakelogs_reminder ON intake_logs(ReminderId);
