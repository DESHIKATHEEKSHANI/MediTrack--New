# MediTrack Reminder

A cross-platform desktop medication reminder application built with **Avalonia UI**, **.NET 8**, **Entity Framework Core**, and **MySQL**.

## Architecture

- **MediTrack.UI** — Avalonia desktop application (Views, ViewModels, App bootstrap)
- **MediTrack.Core** — Domain models, EF Core DbContext, services, and business logic

## Project Structure

```
MediTrack/
├── MediTrack.sln
├── MediTrack.Core/
│   ├── Data/
│   │   └── MediTrackDbContext.cs
│   ├── Models/
│   │   ├── User.cs
│   │   ├── Medication.cs
│   │   └── IntakeLog.cs
│   └── Services/
│       ├── IAuthService.cs
│       ├── AuthService.cs
│       ├── IMedicationService.cs
│       ├── MedicationService.cs
│       ├── IIntakeLogService.cs
│       ├── IntakeLogService.cs
│       ├── IReminderEngine.cs
│       └── ReminderEngine.cs
└── MediTrack.UI/
    ├── App.axaml
    ├── App.axaml.cs
    ├── Program.cs
    ├── app.manifest
    ├── Services/
    │   └── TrayService.cs
    ├── ViewModels/
    │   ├── MainViewModel.cs
    │   ├── SplashViewModel.cs
    │   ├── LoginViewModel.cs
    │   ├── DashboardViewModel.cs
    │   └── AlertViewModel.cs
    └── Views/
        ├── MainWindow.axaml
        ├── MainWindow.axaml.cs
        ├── SplashView.axaml
        ├── SplashView.axaml.cs
        ├── LoginView.axaml
        ├── LoginView.axaml.cs
        ├── DashboardView.axaml
        ├── DashboardView.axaml.cs
        ├── AlertWindow.axaml
        └── AlertWindow.axaml.cs
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/)
- Visual Studio 2022 (optional, with Avalonia extension)

## CLI Setup Commands

Create the solution and projects:

```powershell
# Create solution folder
cd d:\Assignments\MediTrack

# Create solution
dotnet new sln -n MediTrack

# Create Core class library
dotnet new classlib -n MediTrack.Core -f net8.0 --implicit-usings enable
dotnet sln add MediTrack.Core\MediTrack.Core.csproj

# Create Avalonia desktop app with MVVM
dotnet new avalonia.mvvm -n MediTrack.UI -f net8.0
dotnet sln add MediTrack.UI\MediTrack.UI.csproj
```

Install NuGet packages:

```powershell
# Core packages
cd MediTrack.Core
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.4
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 8.0.2
dotnet add package BCrypt.Net-Next --version 4.0.3
dotnet add package CommunityToolkit.Mvvm --version 8.2.2
cd ..

# UI packages
cd MediTrack.UI
dotnet add package Avalonia --version 11.0.10
dotnet add package Avalonia.Desktop --version 11.0.10
dotnet add package Avalonia.Themes.Fluent --version 11.0.10
dotnet add package Avalonia.Fonts.Inter --version 11.0.10
dotnet add package Avalonia.ReactiveUI --version 11.0.10
dotnet add package CommunityToolkit.Mvvm --version 8.2.2
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.4
dotnet add package Microsoft.Extensions.DependencyInjection --version 8.0.0

# Add project reference
dotnet add reference ..\MediTrack.Core\MediTrack.Core.csproj
cd ..
```

## Database Configuration

1. Create a MySQL database named `meditrack`.
2. Update the connection string in `MediTrack.UI/App.axaml.cs`:

```csharp
var connectionString = "server=localhost;database=meditrack;user=root;password=YOUR_PASSWORD;";
```

3. Run EF Core migrations to create the schema:

```powershell
cd MediTrack.UI
dotnet ef migrations add InitialCreate --project ..\MediTrack.Core\MediTrack.Core.csproj
dotnet ef database update --project ..\MediTrack.Core\MediTrack.Core.csproj
cd ..
```

> Note: Ensure the EF Core CLI tools are installed: `dotnet tool install --global dotnet-ef`

## Running the Application

```powershell
cd MediTrack.UI
dotnet run
```

Or open `MediTrack.sln` in Visual Studio and press **F5**.

## Feature Overview

| Module | Description |
|--------|-------------|
| **Authentication** | Register / Login with bcrypt password hashing. Session state managed via `IAuthService`. |
| **Medicine Management** | Add, edit, delete, and view medications with dosage, instructions, and weekday/time schedules. |
| **Background Reminders** | `ReminderEngine` polls every 30 seconds against the system clock. Alerts appear as non-intrusive desktop pop-ups with "Taken" and "Dismiss" actions. |
| **Tray Integration** | Closing the main window minimizes to the system tray. Right-click the tray icon to restore or exit. |
| **Adherence Tracking** | Every user action is logged to an immutable `IntakeLog` table. Weekly compliance percentage is calculated and displayed on the dashboard. |

## Key Design Decisions

- **MVVM Pattern**: Views are completely decoupled from ViewModels via Avalonia `DataTemplates` declared in `App.axaml`.
- **Dependency Injection**: `Microsoft.Extensions.DependencyInjection` registers all services and ViewModels as singletons.
- **Navigation**: `MainViewModel` hosts a `ContentControl` that swaps the current ViewModel. The splash screen auto-transitions to login after 2 seconds.
- **Cross-Platform Target**: Avalonia UI targets Windows primarily but the codebase supports Linux/macOS with minimal changes.

## License

MIT
