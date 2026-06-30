# MediTrack — Full Technical Documentation

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Solution & Folder Structure](#2-solution--folder-structure)
3. [Technology Stack & NuGet Packages](#3-technology-stack--nuget-packages)
4. [Database Schema & Relationships](#4-database-schema--relationships)
5. [Core Layer — Models](#5-core-layer--models)
6. [Core Layer — Data Access (EF Core)](#6-core-layer--data-access-ef-core)
7. [Core Layer — Services](#7-core-layer--services)
8. [UI Layer — Architecture & Startup](#8-ui-layer--architecture--startup)
9. [UI Layer — Views (Screens)](#9-ui-layer--views-screens)
10. [UI Layer — ViewModels](#10-ui-layer--viewmodels)
11. [UI Layer — UI-Only Models](#11-ui-layer--ui-only-models)
12. [UI Layer — Converters](#12-ui-layer--converters)
13. [Security Implementation](#13-security-implementation)
14. [Validation Rules](#14-validation-rules)
15. [Reminder & Notification System](#15-reminder--notification-system)
16. [Navigation System](#16-navigation-system)
17. [Data Flow — End-to-End Examples](#17-data-flow--end-to-end-examples)
18. [Key Design Decisions](#18-key-design-decisions)

---

## 1. Project Overview

**MediTrack** is a Windows desktop application (WPF, .NET 9) that helps users manage medications, schedule reminders, and track daily intake. It stores all data in a MySQL database via Entity Framework Core.

**Core capabilities:**
- User account registration, login, profile editing, password change
- Add/edit/archive/delete medications with dosage, type, and instructions
- Per-medication reminder schedules: multiple daily times, active days of the week, start/end dates
- Background reminder engine that fires popup alert notifications at scheduled times with snooze support
- Daily schedule view showing Pending / Taken / Missed / Dismissed doses
- 7-day adherence tracking with bar chart
- Pill inventory with low-stock alerts
- Full intake log history with status filtering

---

## 2. Solution & Folder Structure

```
MediTrack.sln
│
├── MediTrack.Core/                     ← Class library (business logic + data)
│   ├── Data/
│   │   ├── MediTrackDbContext.cs        ← EF Core DbContext
│   │   └── MediTrackDbContextFactory.cs ← Design-time factory for migrations
│   ├── Migrations/                     ← EF Core migration files
│   ├── Models/
│   │   ├── User.cs
│   │   ├── Medication.cs
│   │   ├── MedicationSchedule.cs
│   │   ├── MedicationInventory.cs
│   │   ├── MedicationSnooze.cs
│   │   ├── Reminder.cs
│   │   └── IntakeLog.cs
│   └── Services/
│       ├── IAuthService.cs / AuthService.cs
│       ├── IMedicationService.cs / MedicationService.cs
│       ├── IIntakeLogService.cs / IntakeLogService.cs
│       └── IReminderEngine.cs / ReminderEngine.cs
│
└── MediTrack.UI/                       ← WPF application
    ├── App.xaml / App.xaml.cs          ← DI setup, startup, event wiring
    ├── Assets/
    │   └── logo-meditrack.png
    ├── Converters/
    │   ├── StringToVisibilityConverter.cs
    │   ├── InvertedBooleanToVisibilityConverter.cs
    │   └── TagActiveFilterConverter.cs  ← EqualityToBrushConverter + EqualityToForegroundConverter
    ├── Models/                         ← UI-only models (not persisted)
    │   ├── NavItem.cs
    │   ├── ScheduleItem.cs
    │   ├── WeeklyBarItem.cs
    │   ├── ReminderSetting.cs
    │   ├── ReminderTimeEntry.cs
    │   └── DayOfWeekCheckItem.cs
    ├── Services/
    │   ├── INavigationService.cs / NavigationService.cs
    │   └── TrayService.cs
    ├── ViewModels/
    │   ├── MainViewModel.cs
    │   ├── SplashViewModel.cs
    │   ├── LoginViewModel.cs
    │   ├── DashboardViewModel.cs
    │   ├── MyMedicinesViewModel.cs
    │   ├── ScheduleViewModel.cs
    │   ├── RemindersViewModel.cs
    │   ├── HistoryViewModel.cs
    │   ├── SettingsViewModel.cs
    │   └── AlertViewModel.cs
    └── Views/
        ├── MainWindow.xaml(.cs)
        ├── SplashView.xaml(.cs)
        ├── LoginView.xaml(.cs)
        ├── DashboardView.xaml(.cs)
        ├── MyMedicinesView.xaml(.cs)
        ├── ScheduleView.xaml(.cs)
        ├── RemindersView.xaml(.cs)
        ├── HistoryView.xaml(.cs)
        ├── SettingsView.xaml(.cs)
        └── AlertWindow.xaml(.cs)
```

---

## 3. Technology Stack & NuGet Packages

| Component | Technology |
|---|---|
| UI Framework | WPF (Windows Presentation Foundation) |
| Language | C# 13, .NET 9.0 |
| MVVM Toolkit | CommunityToolkit.Mvvm 8.2.2 |
| ORM | Entity Framework Core 9.0.0 |
| Database Driver | Pomelo.EntityFrameworkCore.MySql 9.0.0-preview |
| Database | MySQL (via `meditrack-db`) |
| Password Hashing | BCrypt.Net-Next 4.0.3 |
| Charts | LiveCharts.Wpf 0.9.7 |
| Dependency Injection | Microsoft.Extensions.DependencyInjection 9.0.0 |
| System Tray | System.Windows.Forms (NotifyIcon) |

**Connection string** (hardcoded in App.xaml.cs and MediTrackDbContextFactory.cs):
```
server=localhost;database=meditrack-db;user=root;password=1234;
```

---

## 4. Database Schema & Relationships

### 4.1 Entity-Relationship Overview

```
users ──< medications ──< medication_schedules
                      │
                      ├──── medication_inventory  (1:1)
                      ├──── medication_snooze     (1:1)
                      │
                      └──< reminders >──< intake_logs
                                │              │
                                └──────────────┤
                                               └── users
```

### 4.2 Table Definitions

#### `users`
| Column | Type | Constraints |
|---|---|---|
| Id | INT | PK, AUTO_INCREMENT |
| Username | VARCHAR(50) | NOT NULL, UNIQUE |
| Email | VARCHAR(100) | NOT NULL, UNIQUE |
| PasswordHash | VARCHAR(256) | NOT NULL (BCrypt hash) |
| FullName | VARCHAR(100) | NOT NULL |
| CreatedAt | DATETIME | NOT NULL, DEFAULT CURRENT_TIMESTAMP |
| LastLoginAt | DATETIME | NULL |
| IsActive | TINYINT(1) | NOT NULL, DEFAULT 1 |

#### `medications`
| Column | Type | Constraints |
|---|---|---|
| Id | INT | PK, AUTO_INCREMENT |
| UserId | INT | NOT NULL, FK → users.Id CASCADE |
| OfficialName | VARCHAR(150) | NOT NULL |
| DisplayName | VARCHAR(150) | NULL |
| DosageValue | DOUBLE | NOT NULL |
| DosageUnit | VARCHAR(20) | NOT NULL |
| MedicineType | VARCHAR(30) | NOT NULL, DEFAULT 'Tablet' |
| IntakeInstructions | VARCHAR(500) | NOT NULL, DEFAULT '' |
| IsActive | TINYINT(1) | NOT NULL, DEFAULT 1 (soft delete) |
| IsArchived | TINYINT(1) | NOT NULL, DEFAULT 0 |
| CreatedAt | DATETIME | NOT NULL, DEFAULT CURRENT_TIMESTAMP |
| UpdatedAt | DATETIME | NULL |

#### `medication_schedules`
One row per reminder time. A medication taken twice daily has two rows.

| Column | Type | Constraints |
|---|---|---|
| Id | INT | PK, AUTO_INCREMENT |
| MedicationId | INT | NOT NULL, FK → medications.Id CASCADE |
| ReminderTime | TIME | NOT NULL (e.g. 08:00:00) |
| WeekdaySchedule | JSON | NOT NULL (e.g. [1,2,3,4,5] — Mon to Fri) |
| MealTiming | VARCHAR(30) | NOT NULL, DEFAULT 'Any Time' |
| StartDate | DATE | NULL |
| EndDate | DATE | NULL |
| IsOngoing | TINYINT(1) | NOT NULL, DEFAULT 1 |
| IsActive | TINYINT(1) | NOT NULL, DEFAULT 1 |

> **WeekdaySchedule JSON values:** 0=Sunday, 1=Monday, 2=Tuesday, 3=Wednesday, 4=Thursday, 5=Friday, 6=Saturday

#### `medication_inventory`
| Column | Type | Constraints |
|---|---|---|
| Id | INT | PK, AUTO_INCREMENT |
| MedicationId | INT | NOT NULL, UNIQUE, FK → medications.Id CASCADE |
| RemainingPills | INT | NULL |
| LowStockAlertAt | INT | NULL |

#### `medication_snooze`
| Column | Type | Constraints |
|---|---|---|
| Id | INT | PK, AUTO_INCREMENT |
| MedicationId | INT | NOT NULL, UNIQUE, FK → medications.Id CASCADE |
| SnoozeMinutes | INT | NOT NULL, DEFAULT 5 |

#### `reminders`
Daily generated instances, one per medication_schedule per day.

| Column | Type | Constraints |
|---|---|---|
| Id | INT | PK, AUTO_INCREMENT |
| MedicationId | INT | NOT NULL, FK → medications.Id CASCADE |
| ScheduleId | INT | NOT NULL, FK → medication_schedules.Id CASCADE |
| ScheduledDateTime | DATETIME | NOT NULL |
| Status | INT | NOT NULL, DEFAULT 1 (0=Taken, 1=Pending, 2=Missed, 3=Snoozed) |
| SnoozeUntil | DATETIME | NULL |
| CreatedAt | DATETIME | NOT NULL, DEFAULT CURRENT_TIMESTAMP |

#### `intake_logs`
Audit trail — written when user acts on a reminder.

| Column | Type | Constraints |
|---|---|---|
| Id | INT | PK, AUTO_INCREMENT |
| ReminderId | INT | NOT NULL, FK → reminders.Id CASCADE |
| UserId | INT | NOT NULL, FK → users.Id CASCADE |
| MedicationId | INT | NOT NULL, FK → medications.Id CASCADE |
| ActionTimestamp | DATETIME | NULL |
| Status | INT | NOT NULL (0=Taken, 2=Missed) |
| LoggedAt | DATETIME | NOT NULL, DEFAULT CURRENT_TIMESTAMP |

### 4.3 Indexes

```sql
idx_medications_user     ON medications(UserId)
idx_schedules_med        ON medication_schedules(MedicationId)
idx_reminders_med        ON reminders(MedicationId)
idx_reminders_schedule   ON reminders(ScheduleId)
idx_reminders_status     ON reminders(Status)
idx_reminders_datetime   ON reminders(ScheduledDateTime)
idx_intakelogs_user      ON intake_logs(UserId)
idx_intakelogs_med       ON intake_logs(MedicationId)
idx_intakelogs_reminder  ON intake_logs(ReminderId)
```

---

## 5. Core Layer — Models

### `User`
Maps to `users` table.
```
Id, Username, Email, PasswordHash, FullName, CreatedAt, LastLoginAt, IsActive
Navigation: Medications (1:many), IntakeLogs (1:many)
```

### `Medication`
Maps to `medications` table. Also contains **[NotMapped]** "flat" convenience properties that are populated in-memory from related tables by `HydrateFlatProperties()`:

| [NotMapped] Property | Source |
|---|---|
| `Frequency` | Derived from number of schedule rows |
| `ReminderTimes` | Comma-separated `ReminderTime` values from all active schedules |
| `MealTiming` | First active schedule's `MealTiming` |
| `StartDate` | First active schedule's `StartDate` |
| `EndDate` | First active schedule's `EndDate` |
| `IsOngoing` | First active schedule's `IsOngoing` |
| `RemainingPills` | `medication_inventory.RemainingPills` |
| `LowStockAlertAt` | `medication_inventory.LowStockAlertAt` |
| `SnoozeMinutes` | `medication_snooze.SnoozeMinutes` |
| `WeekdaySchedule` | Union of all active schedules' `WeekdaySchedule` arrays |
| `ScheduledTime` | First active schedule's `ReminderTime` |

Navigation: `User`, `Schedules` (1:many), `Inventory` (1:1), `Snooze` (1:1), `Reminders` (1:many), `IntakeLogs` (1:many)

### `MedicationSchedule`
Maps to `medication_schedules`. One row per dose time.
```
Id, MedicationId, ReminderTime (TimeSpan→TIME), WeekdaySchedule (DayOfWeek[]→JSON),
MealTiming, StartDate, EndDate, IsOngoing, IsActive
Navigation: Medication, Reminders (1:many)
```

### `MedicationInventory`
Maps to `medication_inventory`. 1:1 with Medication.
```
Id, MedicationId, RemainingPills (int?), LowStockAlertAt (int?)
Navigation: Medication
```

### `MedicationSnooze`
Maps to `medication_snooze`. 1:1 with Medication.
```
Id, MedicationId, SnoozeMinutes (default 5)
Navigation: Medication
```

### `Reminder` + `ReminderStatus` enum
Maps to `reminders`. Generated daily per schedule row.
```csharp
enum ReminderStatus { Taken = 0, Pending = 1, Missed = 2, Snoozed = 3 }

Reminder: Id, MedicationId, ScheduleId, ScheduledDateTime, Status, SnoozeUntil, CreatedAt
Navigation: Medication, Schedule (MedicationSchedule), IntakeLogs (1:many)
```

### `IntakeLog` + `IntakeStatus` enum
Maps to `intake_logs`. Audit record created when user acts.
```csharp
enum IntakeStatus { Taken = 0, Pending = 1, Missed = 2 }

IntakeLog: Id, ReminderId, UserId, MedicationId, ActionTimestamp, Status, LoggedAt
Navigation: Reminder, User, Medication
```

---

## 6. Core Layer — Data Access (EF Core)

### `MediTrackDbContext`

**DbSets:**
```csharp
DbSet<User>                Users
DbSet<Medication>          Medications
DbSet<MedicationSchedule>  MedicationSchedules
DbSet<MedicationInventory> MedicationInventories
DbSet<MedicationSnooze>    MedicationSnoozes
DbSet<Reminder>            Reminders
DbSet<IntakeLog>           IntakeLogs
```

**Key OnModelCreating configurations:**

1. **Explicit table names** (snake_case matching SQL schema):
   ```csharp
   modelBuilder.Entity<User>().ToTable("users");
   modelBuilder.Entity<Medication>().ToTable("medications");
   modelBuilder.Entity<MedicationSchedule>().ToTable("medication_schedules");
   modelBuilder.Entity<MedicationInventory>().ToTable("medication_inventory");
   modelBuilder.Entity<MedicationSnooze>().ToTable("medication_snooze");
   modelBuilder.Entity<Reminder>().ToTable("reminders");
   modelBuilder.Entity<IntakeLog>().ToTable("intake_logs");
   ```

2. **WeekdaySchedule JSON serialization** (DayOfWeek[] ↔ JSON int array):
   ```csharp
   entity.Property(e => e.WeekdaySchedule)
       .HasColumnType("json")
       .HasConversion(
           v => JsonSerializer.Serialize(v.Select(d => (int)d).ToArray(), ...),
           v => JsonSerializer.Deserialize<int[]>(v, ...)!.Select(i => (DayOfWeek)i).ToArray());
   ```

3. **Unique indexes** on `Username`, `Email` (users), `MedicationId` (inventory, snooze)

4. **Cascade deletes** on all FK relationships

5. **All FK relationships** explicitly configured with `.HasOne()/.WithMany()/.HasForeignKey()`

**Note on [NotMapped] properties:**  
Properties like `WeekdaySchedule`, `ScheduledTime`, `ReminderTimes` etc. on `Medication` are marked `[NotMapped]` — EF Core ignores them during queries and saves. They are filled in-memory by `HydrateFlatProperties()` and written back to related tables by `SyncRelatedEntities()` / the new `UpdateAsync` approach.

---

## 7. Core Layer — Services

### `IAuthService` / `AuthService`

**Interface:**
```csharp
Task<User?> RegisterAsync(string username, string email, string password, string fullName)
Task<User?> LoginAsync(string usernameOrEmail, string password)
Task LogoutAsync()
Task<bool> UpdateProfileAsync(int userId, string fullName, string email)
Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
User? CurrentUser { get; }
event EventHandler? AuthStateChanged
```

**Implementation details:**
- `RegisterAsync`: checks for duplicate username OR email → hashes password with BCrypt → inserts user → sets `CurrentUser` → fires `AuthStateChanged`
- `LoginAsync`: queries by username OR email → verifies BCrypt hash → updates `LastLoginAt` → sets `CurrentUser` → fires `AuthStateChanged`
- `LogoutAsync`: clears `CurrentUser` → fires `AuthStateChanged`
- `UpdateProfileAsync`: checks new email not taken by another user → updates FullName + Email in DB and in `CurrentUser` in-memory
- `ChangePasswordAsync`: verifies current password with BCrypt → hashes new password → saves
- `CurrentUser` is stored as an in-memory singleton (cleared on logout)

---

### `IMedicationService` / `MedicationService`

**Interface:**
```csharp
Task<Medication?> GetByIdAsync(int id)
Task<IEnumerable<Medication>> GetUserMedicationsAsync(int userId)
Task<Medication> AddAsync(Medication medication)
Task<Medication?> UpdateAsync(Medication medication)
Task<bool> DeleteAsync(int id)
Task<IEnumerable<Medication>> GetTodayScheduleAsync(int userId)
```

**Key internal helpers:**

**`HydrateFlatProperties(Medication med)`** — Populates the `[NotMapped]` flat properties from the navigation collections after a DB read:
```
WeekdaySchedule ← union of all active Schedules[].WeekdaySchedule
ScheduledTime   ← first active Schedule.ReminderTime
ReminderTimes   ← comma join of Schedules[].ReminderTime.ToString("hh\\:mm")
Frequency       ← "Once daily" / "Twice daily" / "Three times daily" / "Custom"
MealTiming      ← first active Schedule.MealTiming
StartDate/EndDate/IsOngoing ← first active Schedule.*
RemainingPills/LowStockAlertAt ← Inventory.*
SnoozeMinutes   ← Snooze.SnoozeMinutes
```

**`UpdateAsync`** — Works directly on the EF-tracked `existing` entity (not a detached copy):
1. Updates scalar fields on `existing`
2. `existing.Schedules.Clear()` → EF marks old rows as Deleted
3. Iterates parsed `ReminderTimes` → adds new `MedicationSchedule` rows with correct `WeekdaySchedule`, dates, meal timing
4. Updates `existing.Inventory` (creates if null)
5. Updates `existing.Snooze` (creates if null)
6. `SaveChangesAsync()` → atomic DEL+INSERT for schedules, UPDATE for inventory/snooze

**`DeleteAsync`** — Soft delete: sets `IsActive = false`, never physically deletes.

**`GetTodayScheduleAsync`** — Returns medications that have at least one active schedule with today's `DayOfWeek` in its `WeekdaySchedule` array.

---

### `IIntakeLogService` / `IntakeLogService`

**Interface:**
```csharp
event EventHandler<Medication>? LowStockAlertTriggered
Task<IntakeLog> LogActionAsync(int userId, int medicationId, DateTime scheduledDateTime, IntakeStatus status)
Task<IEnumerable<IntakeLog>> GetUserLogsAsync(int userId, DateTime? from, DateTime? to)
Task<IEnumerable<IntakeLog>> GetTodayLogsAsync(int userId)
Task<double> GetWeeklyComplianceAsync(int userId)
Task<List<double>> GetWeeklyAdherenceAsync(int userId)
Task GenerateScheduledLogsAsync(int userId)
```

**`LogActionAsync`** — The most complex method:
1. Looks for existing `Reminder` with matching `MedicationId` + `ScheduledDateTime`
2. If not found: queries `medication_schedules` to find a valid `ScheduleId` (by `ReminderTime == scheduledDateTime.TimeOfDay`), falls back to any active schedule if no exact match
3. Creates new `Reminder` with correct `ScheduleId` (never uses 0 → would fail FK)
4. Updates Reminder status (Taken or Missed)
5. Creates or updates `IntakeLog` record
6. Calls `DecrementPillsAndCheckLowStockAsync` → decrements `RemainingPills` → fires `LowStockAlertTriggered` if threshold reached

**`GenerateScheduledLogsAsync`** — Called on Dashboard load:
- Loads all active medications with their schedules
- For each active schedule whose `WeekdaySchedule` contains today, and within `StartDate`/`EndDate` range:
  - Creates a `Reminder` row (Status=Pending) if one doesn't exist for today's scheduled time
  - Creates a corresponding `IntakeLog` row (Status=Pending)

**`GetWeeklyAdherenceAsync`** — Returns 7 `double` values (oldest → newest), each the percentage of Taken logs for that day.

---

### `IReminderEngine` / `ReminderEngine`

**Interface:**
```csharp
event EventHandler<ReminderEventArgs>? ReminderTriggered
bool IsRunning { get; }
void Start()
void Stop()
void SnoozeReminder(int medicationId, DateTime scheduledTime, int minutes)
```

**Implementation:**
- Timer fires every **30 seconds**
- On each tick (`OnTimerElapsed`):
  1. Gets all active medications for `_currentUserId` with their schedules and snooze settings
  2. For each active schedule whose `WeekdaySchedule` contains today's `DayOfWeek`:
     - Checks if `scheduledToday` falls within a **±2-minute window** around `now`
     - Checks start/end date bounds
     - Checks if a non-Pending Reminder already exists (i.e. user already acted)
     - If not acted: fires `ReminderTriggered` event
  3. Re-fires any **snoozed reminders** whose `SnoozeUntil` has passed
- `SnoozeReminder`: adds to thread-safe `_snoozed` list (locks on `_snoozed`)
- `SetUser(int userId)`: sets the current user after login (called from App.xaml.cs via `AuthStateChanged` event)

---

## 8. UI Layer — Architecture & Startup

### MVVM Pattern

All UI code follows strict MVVM via **CommunityToolkit.Mvvm**:
- `[ObservableProperty]` generates backing fields + `INotifyPropertyChanged` + partial `OnXxxChanged()` hooks
- `[RelayCommand]` generates `ICommand` / `IAsyncCommand` wrappers
- All ViewModels extend `ObservableObject`

### Dependency Injection (`App.xaml.cs`)

All services and ViewModels registered as **singletons** in `Microsoft.Extensions.DependencyInjection`:

```csharp
// Services
services.AddDbContext<MediTrackDbContext>(options => options.UseMySql(...));
services.AddSingleton<INavigationService, NavigationService>();
services.AddSingleton<IAuthService, AuthService>();
services.AddSingleton<IMedicationService, MedicationService>();
services.AddSingleton<IIntakeLogService, IntakeLogService>();
services.AddSingleton<IReminderEngine, ReminderEngine>();

// ViewModels
services.AddSingleton<MainViewModel>();
services.AddSingleton<LoginViewModel>();
services.AddSingleton<SplashViewModel>();
services.AddSingleton<DashboardViewModel>();
services.AddSingleton<MyMedicinesViewModel>();
services.AddSingleton<ScheduleViewModel>();
services.AddSingleton<RemindersViewModel>();
services.AddSingleton<HistoryViewModel>();
services.AddSingleton<SettingsViewModel>();
```

### Startup Sequence

```
OnStartup()
  │
  ├── Build DI container
  ├── Create MainWindow (contains ContentControl bound to CurrentViewModel)
  ├── NavigateTo(SplashViewModel) → 2-second delay → NavigateTo(LoginViewModel)
  ├── Wire AuthStateChanged:
  │     User logged in  → engine.SetUser(userId) + engine.Start()
  │     User logged out → engine.Stop()
  ├── Wire ReminderTriggered:
  │     → Play SystemSounds.Exclamation
  │     → Create AlertViewModel (with close + snooze callbacks)
  │     → Show AlertWindow (positioned bottom-right, always-on-top)
  ├── Wire LowStockAlertTriggered:
  │     → Play SystemSounds.Hand
  │     → Show MessageBox warning
  └── Wire TrayService (intercepts window close → hide to tray)
```

### View Resolution

`App.xaml` defines DataTemplates that map each ViewModel type to its View:
```xml
<DataTemplate DataType="{x:Type vm:DashboardViewModel}">
    <v:DashboardView />
</DataTemplate>
<!-- ... one per screen ... -->
```
When `NavigationService.CurrentViewModel` changes, the `ContentControl` in `MainWindow` automatically renders the correct View.

### System Tray (`TrayService`)

- Intercepts `Window.Closing` event → hides window instead of closing
- `NotifyIcon` context menu: "Open MediTrack" (show window), "Exit" (full shutdown)
- Icon: `SystemIcons.Application`

---

## 9. UI Layer — Views (Screens)

| View | ViewModel | Purpose |
|---|---|---|
| `SplashView` | `SplashViewModel` | Branded 2-second loading screen |
| `LoginView` | `LoginViewModel` | Sign in / Create account (dual-mode) |
| `DashboardView` | `DashboardViewModel` | Home: stats, today's doses, chart, quick actions |
| `MyMedicinesView` | `MyMedicinesViewModel` | Medication inventory with search/filter |
| `ScheduleView` | `ScheduleViewModel` | Day-by-day dose timeline |
| `RemindersView` | `RemindersViewModel` | Reminder rules: times, days, dates, snooze |
| `HistoryView` | `HistoryViewModel` | Intake log history with status filter |
| `SettingsView` | `SettingsViewModel` | Profile edit, password change, sound toggle |
| `AlertWindow` | `AlertViewModel` | Popup notification when reminder fires |

**Sidebar** — All authenticated views share a 240px `#104C7F` left sidebar with:
- Logo + app name
- 6 `ListBox` nav items with SVG icon paths bound to `NavItem.Icon`
- Bottom panel: greeting + Logout button

**All nav items use SVG icon `Path` elements** (`Data="{Binding Icon}"`) — not generic dot ellipses.

---

## 10. UI Layer — ViewModels

### `LoginViewModel`

**State:** `Username`, `Password`, `Email`, `FullName`, `ConfirmPassword`, `ErrorMessage`, `IsLoginMode`

**Commands:**
- `LoginCommand` → validates fields → calls `AuthService.LoginAsync()` → navigates to Dashboard
- `RegisterCommand` → validates all fields including email format + password rules → calls `AuthService.RegisterAsync()`
- `ToggleModeCommand` → switches between login and registration form

**Validation in `RegisterAsync`:**
1. All fields required
2. Email format: regex `^[^@\s]+@[^@\s]+\.[^@\s]{2,}$`
3. Passwords match
4. Password ≥ 6 characters
5. Duplicate username/email (handled by AuthService returning null)

### `DashboardViewModel`

**State:** Today's medication logs, weekly adherence bars, stats counts, next dose info, selected medication for add/edit popup

**Auto-reload:** Subscribes to `NavigationService.Navigated` — reloads when this VM becomes current.

**Key commands:** `MarkTakenCommand`, `MarkMissedCommand`, `ShowAddMedicationCommand`, `ShowEditMedicationCommand`, `SaveMedicationCommand`, `DeleteMedicationCommand`, `NavigateToMyMedicinesCommand`, `RefreshCommand`, `ExpandChartCommand`

**`LoadDataAsync()`:**
1. Calls `GenerateScheduledLogsAsync()` to create today's Pending records
2. Loads today's logs (with Medication navigation included)
3. Builds `TodayLogsTruncated` (first 3), `HasMoreLogs`, `ExtraLogsCount`
4. Loads all medications → `AllMedicines` (first 5 for My Medications section)
5. Computes `TakenToday`, `MissedThisWeek`, `WeeklyCompliance`
6. Builds `WeeklyAdherenceBars` (7 items with color: ≥70%=green, ≥40%=orange, <40%=red)
7. Finds `NextDoseTime`/`NextDoseMedicine` from first upcoming Pending log
8. Builds `RecentActivityLogs` (last 5 acted-on logs, most recent first)

### `MyMedicinesViewModel`

**State:** Full medication list, filtered list, search text, active filter (All/Active/Archived), selected medicine for popup, validation errors

**Auto-reload:** Subscribes to `NavigationService.Navigated`.

**Validation in `ValidateMedicine()`:**
1. `OfficialName` required
2. No duplicate name in existing (non-archived) medicines — case-insensitive in-memory check
3. `DosageValue > 0`

**`SaveMedicineAsync()`:** Calls `MedicationService.AddAsync()` or `UpdateAsync()`, then reloads.

**`ToggleArchiveAsync()`:** Flips `IsArchived`, calls `UpdateAsync()`.

**`DeleteAsync()`:** Soft delete (`IsActive = false`).

### `ScheduleViewModel`

**State:** `SelectedDate`, `ScheduleItems`, progress counts, next medication

**Auto-reload:** Subscribes to `NavigationService.Navigated`.

**`LoadScheduleAsync()`:**
1. Loads all user medications with schedules
2. Filters to those with a schedule active on `SelectedDate`'s day of week, within start/end dates
3. For each medication × each schedule's `ReminderTime`: creates a `ScheduleItem` and looks up its matching log
4. Auto-marks items as Missed where `ScheduledDateTime < now && Status == Pending`
5. Finds `NextMedication` (first upcoming Pending item)

**`ScheduleItem.IsMissed`:** `Status == Pending && ScheduledDateTime < DateTime.Now` — purely computed, no DB write.

### `RemindersViewModel`

**State:** Reminder list (parsed from medication schedules), weekday selector, date pickers, snooze, medications dropdown

**Auto-reload:** Subscribes to `NavigationService.Navigated`. Also re-syncs `NotificationsEnabled` from `reminderEngine.IsRunning` each time.

**`LoadRemindersAsync()`:** Reads medications, calls `HydrateFlatProperties()` internally (via `GetUserMedicationsAsync()`), parses `ReminderTimes` string → one `ReminderSetting` per time per medication. Calls `FormatActiveDays(med.WeekdaySchedule)` to produce human-readable day strings.

**`FormatActiveDays(DayOfWeek[])`:**
- All 7 → "Every day"
- Mon–Fri only → "Weekdays"
- Sat–Sun only → "Weekends"
- Other → comma-joined abbreviations (e.g. "Mon, Wed, Fri")

**`SaveReminderAsync()`:**
1. Validates: medicine selected, at least one time, at least one day
2. Loads medication via `GetByIdAsync()`
3. Sets `[NotMapped]` properties: `ReminderTimes`, `ScheduledTime`, `Frequency`, `SnoozeMinutes`, `WeekdaySchedule`, `StartDate`, `EndDate`, `IsOngoing`
4. Calls `MedicationService.UpdateAsync()` which syncs to `medication_schedules` + `medication_snooze`

**`OnNotificationsEnabledChanged()`:** Calls `_reminderEngine.Start()` or `_reminderEngine.Stop()` immediately.

### `HistoryViewModel`

**State:** All intake logs, filtered logs, active filter

**Auto-reload:** Subscribes to `NavigationService.Navigated`.

**Filter values:** "All", "Taken", "Skipped" (Dismissed), "Pending"

### `SettingsViewModel`

**State:** User profile display, edit fields, password change fields, error/success messages

**Commands:** `EditProfileCommand`, `SaveProfileCommand`, `CancelEditProfileCommand`, `ShowChangePasswordCommand`, `SavePasswordCommand`, `CancelChangePasswordCommand`, `LogoutCommand`

**`SaveProfileAsync()`:** Validates name/email not empty → calls `AuthService.UpdateProfileAsync()` → updates in-memory display properties.

**`SavePasswordAsync()`:** Validates current password not empty, new password ≥ 6 chars, confirmation matches → calls `AuthService.ChangePasswordAsync()`.

### `AlertViewModel`

**State:** `MedicationName`, `DosageInfo`, `SnoozeMinutes`

**Constructor:** Takes `IIntakeLogService`, `Medication`, `DateTime scheduledTime`, `Action? closeAction`. Has a **parameterless constructor** (required for WPF type scanning).

**Commands:**
- `TakenCommand` → `LogActionAsync(..., Taken)` → invokes `closeAction`
- `MissedCommand` → `LogActionAsync(..., Missed)` → invokes `closeAction`
- `SnoozeCommand` → invokes `OnSnooze` callback (calls `reminderEngine.SnoozeReminder()`) → invokes `closeAction`

---

## 11. UI Layer — UI-Only Models

### `NavItem : ObservableObject`
```csharp
string Label     // Display name
string Icon      // SVG path data string
bool IsSelected  // Highlighted state (bound via DataTrigger)
```

### `ScheduleItem`
Wraps one dose slot for the Schedule view.
```csharp
int MedicationId
string MedicationName, DisplayName
TimeSpan ScheduledTime
DateTime Date
DateTime ScheduledDateTime  // Computed: Date.Date + ScheduledTime
string Dosage, Instructions
IntakeStatus Status
bool IsTaken    // Status == Taken
bool IsPending  // Status == Pending && !IsMissed
bool IsDismissed // Status == Dismissed
bool IsMissed   // Status == Pending && ScheduledDateTime < DateTime.Now
string StatusText
```

### `WeeklyBarItem`
One column in the 7-day adherence chart.
```csharp
double Value            // 0–100 percentage
string Label            // "Today", "Yesterday", or "Mon" etc.
double BarHeightSmall   // Value * 1.2 (compact chart)
double BarHeightLarge   // Value * 3.6 (expanded chart)
string BarColor         // ≥70% → "#4CAF50", ≥40% → "#FF9800", else "#D32F2F"
```

### `ReminderSetting`
UI-only representation of one reminder time for one medication.
```csharp
int Id, int? MedicationId
string MedicationName
bool IsEnabled
string Time      // HH:mm format
string Frequency // "Daily", "Twice daily" etc.
int SnoozeMinutes
string ActiveDays // "Every day" / "Weekdays" / "Mon, Wed, Fri" etc.
```

### `ReminderTimeEntry : ObservableObject`
One time entry in the Add/Edit Reminder popup.
```csharp
int Hour (1–12), int Minute (0–59), string AmPm ("AM"/"PM")
string To24HourString()           // Converts to "HH:mm"
static ReminderTimeEntry From24HourString(string time24) // Parses back
```

### `DayOfWeekCheckItem : ObservableObject`
One day toggle in the weekday selector.
```csharp
DayOfWeek Day
string ShortLabel  // "Mon", "Tue" etc.
bool IsSelected    // Bound to ToggleButton.IsChecked
```

---

## 12. UI Layer — Converters

| Converter | Input → Output | Usage |
|---|---|---|
| `StringToVisibilityConverter` | `null/empty → Collapsed`, non-empty → `Visible` | Error messages, optional labels |
| `InvertedBooleanToVisibilityConverter` | `true → Collapsed`, `false → Visible` | Hide elements when flag is true |
| `EqualityToBrushConverter` (MultiValue) | Two values equal → `#104C7F`, else `Transparent` | Active filter button background |
| `EqualityToForegroundConverter` (MultiValue) | Two values equal → `White`, else `#888888` | Active filter button text color |

---

## 13. Security Implementation

### Password Hashing — BCrypt

**Library:** `BCrypt.Net-Next` v4.0.3

**Registration:**
```csharp
user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
// Produces: "$2a$11$..." (bcrypt with work factor 11)
```

**Login verification:**
```csharp
BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)
// Returns true only if password matches the stored hash
```

**Change password:**
```csharp
// Step 1: verify current password
if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash)) return false;
// Step 2: hash new password
user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
```

BCrypt automatically embeds a random **salt** in the hash — no separate salt storage needed. The work factor (11 by default) makes brute-force attacks computationally expensive.

### Session Management

- `AuthService.CurrentUser` is an in-memory singleton reference — **no tokens, no cookies**
- Cleared on `LogoutAsync()`
- Surviving an app crash clears it automatically (in-memory only)
- `AuthStateChanged` event notifies the ReminderEngine to start/stop accordingly

---

## 14. Validation Rules

### Account Registration

| Rule | Message |
|---|---|
| Any field empty | "All fields are required." |
| Email doesn't match `^[^@\s]+@[^@\s]+\.[^@\s]{2,}$` | "Please enter a valid email address (e.g. user@example.com)." |
| Passwords don't match | "Passwords do not match." |
| Password < 6 characters | "Password must be at least 6 characters." |
| Username or email already taken (AuthService returns null) | "Username or email already exists." |

### Add Medicine

| Rule | Error field | Message |
|---|---|---|
| Name empty | OfficialName | "Medicine name is required." |
| Same name exists in non-archived medicines | OfficialName | "A medicine with this name already exists." |
| Dosage ≤ 0 | DosageValue | "Dose amount must be greater than 0." |

### Add/Edit Reminder

| Rule | Error key | Message |
|---|---|---|
| No medicine selected | Medication | "Please select a medicine." |
| No reminder times | Times | "At least one reminder time is required." |
| No days selected | Weekdays | "Please select at least one day." |

### Update Profile

| Rule | Message |
|---|---|
| Name or email empty | "Name and email are required." |
| Email already used by another account | "That email is already in use by another account." |

### Change Password

| Rule | Message |
|---|---|
| Current password empty | "Current password is required." |
| New password < 6 characters | "New password must be at least 6 characters." |
| Confirmation doesn't match | "Passwords do not match." |
| Current password incorrect (BCrypt verify fails) | "Current password is incorrect." |

---

## 15. Reminder & Notification System

### Overview

```
medication_schedules
        │
        ▼ (GenerateScheduledLogsAsync — on Dashboard load)
    reminders (Pending)
        │
        ▼ (ReminderEngine checks every 30s)
    ReminderTriggered event
        │
        ▼ (App.xaml.cs handler)
    AlertWindow + AlertViewModel
        │
        ▼ (User clicks Taken / Missed / Snooze)
    LogActionAsync → intake_logs (Taken/Missed) + reminders (status updated)
```

### Generation (`GenerateScheduledLogsAsync`)

Called every time the Dashboard loads. For each active medication:
- Finds all active `MedicationSchedule` rows
- Filters to schedules where today's `DayOfWeek` is in `WeekdaySchedule`
- Checks start/end date bounds
- Creates a `Reminder` (Pending) + `IntakeLog` (Pending) for each matching schedule × today date, if not already created

### Detection (`ReminderEngine.OnTimerElapsed`)

Every 30 seconds:
- ±2-minute window around now
- Checks `Reminder.Status != Pending` (i.e., already acted) to avoid duplicate alerts
- Also re-fires any snoozed reminders whose `SnoozeUntil ≤ now`

### Alert (`AlertWindow`)

- Size: 360×200, always-on-top (`Topmost="True"`), positioned bottom-right of screen
- Dark theme (`#263238`), medication name in yellow (`#FFEB3B`)
- Three actions: **Taken** (green), **Dismiss/Missed** (orange), **Snooze** (if configured)
- Closing: `closeAction` lambda captures `alertWindow` reference → `alertWindow.Close()` on Dispatcher
- After close: `DataContext = null` (prevents memory leak)

### Snooze

`RemindersViewModel.ShowAddReminder/ShowEditReminder` lets users set snooze duration (5, 10, 15, 30, 60 min) per medication, stored in `medication_snooze.SnoozeMinutes`.

When user clicks Snooze on the alert:
```
AlertViewModel.Snooze() → OnSnooze callback
→ reminderEngine.SnoozeReminder(medicationId, scheduledTime, snoozeMinutes)
→ SnoozedReminder record added to thread-safe _snoozed list with SnoozeUntil = now + minutes
→ OnTimerElapsed re-fires it when SnoozeUntil passes
```

### Low Stock Alerts

`IntakeLogService.DecrementPillsAndCheckLowStockAsync`:
- On every Taken action: `RemainingPills -= 1`
- If `RemainingPills <= LowStockAlertAt`: fires `LowStockAlertTriggered` event
- App.xaml.cs shows a `MessageBox` warning with pill count

---

## 16. Navigation System

### `INavigationService` / `NavigationService`

```csharp
void NavigateTo(object viewModel)  // Sets CurrentViewModel + fires Navigated
object? CurrentViewModel { get; }
event EventHandler? Navigated
```

`NavigateTo()` sets `_currentViewModel` BEFORE firing `Navigated` — so handlers can safely check `CurrentViewModel == this`.

### View Resolution

`MainWindow.xaml` has a `ContentControl` bound to `MainViewModel.CurrentViewModel`. `App.xaml` defines `DataTemplate` entries mapping each ViewModel type to its View — WPF resolves them automatically.

### Navigation Between Screens

Each ViewModel has a sidebar `ListBox` bound to `NavItems`. When `SelectedNavItem` changes, `OnSelectedNavItemChanged` calls `NavigateTo(targetVM)` then **resets `_selectedNavItem` to this screen's own NavItem** (using the backing field directly to avoid re-triggering the handler). This prevents the double-click bug where a "stuck" selection prevents re-navigation.

### Auto-Reload on Navigate

`MyMedicinesViewModel`, `ScheduleViewModel`, `HistoryViewModel`, and `RemindersViewModel` all subscribe to `Navigated`:
```csharp
_navigationService.Navigated += (_, _) =>
{
    if (_navigationService.CurrentViewModel == this)
        _ = LoadXxxAsync();
};
```
Since ViewModels are singletons, this ensures stale data is refreshed whenever the user returns to a screen.

---

## 17. Data Flow — End-to-End Examples

### A. User Registers

```
RegisterAsync("desh", "d@x.com", "pass123", "Deshika")
  → validate (all fields, email format, password length)
  → DB: SELECT COUNT(*) WHERE Username="desh" OR Email="d@x.com" → 0
  → BCrypt.HashPassword("pass123") → "$2a$11$..."
  → INSERT INTO users (Username, Email, PasswordHash, FullName) VALUES (...)
  → CurrentUser = new user; AuthStateChanged fired
  → App.xaml.cs: engine.SetUser(user.Id); engine.Start()
  → NavigateTo(DashboardViewModel) → LoadDataAsync()
```

### B. User Adds a Medication with 2 Daily Times

```
ShowAddMedication() → popup opens with defaults
User fills: "Amoxicillin", "mg", 250, Tablet, "08:00,20:00", Mon–Fri, Start=today, Ongoing=true

SaveMedicationAsync()
  → medication.ReminderTimes = "08:00,20:00"
  → medication.WeekdaySchedule = [Mon,Tue,Wed,Thu,Fri]
  → medication.IsOngoing = true
  
MedicationService.AddAsync(medication)
  → SyncRelatedEntities(medication):
      ParseReminderTimes("08:00,20:00") → [08:00, 20:00]
      days = [1,2,3,4,5]
      medication.Schedules.Add(new MedicationSchedule { ReminderTime=08:00, WeekdaySchedule=[1,2,3,4,5], IsOngoing=true })
      medication.Schedules.Add(new MedicationSchedule { ReminderTime=20:00, WeekdaySchedule=[1,2,3,4,5], IsOngoing=true })
  → _context.Medications.Add(medication)  // entire graph tracked
  → SaveChangesAsync()
    → INSERT medications (Id=5)
    → INSERT medication_schedules (MedicationId=5, ReminderTime=08:00, WeekdaySchedule=[1,2,3,4,5], ...)
    → INSERT medication_schedules (MedicationId=5, ReminderTime=20:00, WeekdaySchedule=[1,2,3,4,5], ...)
```

### C. Reminder Fires and User Clicks "Taken"

```
ReminderEngine.OnTimerElapsed (08:00 ± 2 min, today is Wednesday)
  → SELECT medications WHERE UserId=1 AND IsActive=1 (with Schedules)
  → For Amoxicillin schedule (08:00, WeekdaySchedule contains Wednesday):
      scheduledToday = today.Date + 08:00
      AnyAsync(reminder where MedicationId=5, ScheduleId=X, ScheduledDateTime=today08:00, Status≠Pending) → false
      → ReminderTriggered.Invoke(medication, today08:00)

App.xaml.cs handler:
  → new AlertViewModel(intakeLogService, medication, today08:00, closeAction)
  → new AlertWindow(alertVm).Show()

User clicks "Taken":
  AlertViewModel.TakenAsync()
    → intakeLogService.LogActionAsync(userId=1, medicationId=5, today08:00, Taken)
      → SELECT reminder WHERE MedicationId=5 AND ScheduledDateTime=today08:00 → found (Pending)
      → reminder.Status = Taken
      → SELECT intakelog WHERE ReminderId=X → found (Pending)
      → existing.Status = Taken; existing.ActionTimestamp = UtcNow
      → SaveChangesAsync() → UPDATE reminders, UPDATE intake_logs
      → DecrementPillsAndCheckLowStockAsync(5, Taken) → RemainingPills -= 1
    → _closeAction.Invoke() → alertWindow.Close()
```

---

## 18. Key Design Decisions

| Decision | Rationale |
|---|---|
| **Singleton ViewModels** | Prevents re-creation overhead; requires Navigated reload hooks to avoid stale data |
| **[NotMapped] flat properties on Medication** | Allows existing UI code to use simple field access while actual data is in normalized related tables |
| **`existing.Schedules.Clear()` + re-add in UpdateAsync** | EF Core must observe changes on tracked collections; creating a `ToList()` copy bypasses tracking |
| **`ScheduleId` lookup in LogActionAsync** | Prevents FK constraint error when a Reminder didn't exist before user acted (race between alert firing and GenerateScheduledLogs) |
| **Parameterless constructor on AlertViewModel** | WPF's type-scanning system requires it even though the real constructor always has parameters |
| **`User.Reminders` navigation removed** | `reminders` table has no `UserId` column — EF Core would auto-create a shadow FK causing "Unknown column" MySQL error |
| **Explicit `.ToTable("snake_case")` mappings** | SQL schema uses snake_case; EF Core defaults to PascalCase — `MedicationSchedules` ≠ `medication_schedules` even on case-insensitive MySQL |
| **BCrypt for passwords** | Industry standard; auto-salted; configurable work factor; `Verify()` is timing-safe |
| **30-second timer with ±2-minute window** | Tolerates timer drift; misses are caught in the next tick; app start-up delay is acceptable |
| **Soft deletes on medications** | Preserves history in `reminders` and `intake_logs`; archived state is reversible |
