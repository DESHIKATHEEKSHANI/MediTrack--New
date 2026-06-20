using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediTrack.Core.Models;
using MediTrack.Core.Services;
using MediTrack.UI.Models;
using MediTrack.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MediTrack.UI.ViewModels;

public partial class MyMedicinesViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IMedicationService _medicationService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<Medication> _medicines = new();

    [ObservableProperty]
    private ObservableCollection<Medication> _filteredMedicines = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _activeFilter = "All";

    [ObservableProperty]
    private bool _isPopupOpen;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private Medication _selectedMedicine = new();

    [ObservableProperty]
    private string _welcomeMessage = string.Empty;

    [ObservableProperty]
    private ObservableCollection<NavItem> _navItems = new()
    {
        new NavItem { Label = "Dashboard", Icon = "M3 13h8V3H3v10zm0 8h8v-6H3v6zm10 0h8V11h-8v10zm0-18v6h8V3h-8z" },
        new NavItem { Label = "My Medicines", Icon = "M6 3h12v18H6V3zm2 2v14h8V5H8zm3 3h2v2h-2V8zm0 4h2v2h-2v-2z", IsSelected = true },
        new NavItem { Label = "Schedule", Icon = "M19 3h-1V1h-2v2H8V1H6v2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H5V8h14v11z" },
        new NavItem { Label = "Reminders", Icon = "M18 16v-5c0-3.07-1.64-5.64-4.5-6.32V4c0-.83-.67-1.5-1.5-1.5s-1.5.67-1.5 1.5v.68C7.63 5.36 6 7.92 6 11v5l-2 2v1h16v-1l-2-2zm-5 0h-2v-2h2v2zm0-4h-2V9h2v3z" },
        new NavItem { Label = "History", Icon = "M13 3a9 9 0 0 0-9 9H1l3.89 3.89.07.14L9 12H6c0-3.87 3.13-7 7-7s7 3.13 7 7-3.13 7-7 7c-1.93 0-3.68-.79-4.94-2.06l-1.42 1.42A8.954 8.954 0 0 0 13 21a9 9 0 0 0 0-18z" },
        new NavItem { Label = "Settings", Icon = "M19.14 12.94c.04-.3.06-.61.06-.94 0-.32-.02-.64-.07-.94l2.03-1.58a.49.49 0 0 0 .12-.61l-1.92-3.32a.488.488 0 0 0-.59-.22l-2.39.96c-.5-.38-1.03-.7-1.62-.94l-.36-2.54a.484.484 0 0 0-.48-.41h-3.84c-.24 0-.43.17-.47.41l-.36 2.54c-.59.24-1.13.57-1.62.94l-2.39-.96a.488.488 0 0 0-.59.22L3.16 8.87c-.12.21-.08.47.12.61l2.03 1.58c-.05.3-.07.63-.07.94s.02.64.07.94l-2.03 1.58a.49.49 0 0 0-.12.61l1.92 3.32c.12.22.37.29.59.22l2.39-.96c.5.38 1.03.7 1.62.94l.36 2.54c.05.24.24.41.48.41h3.84c.24 0 .44-.17.47-.41l.36-2.54c.59-.24 1.13-.56 1.62-.94l2.39.96c.22.08.47 0 .59-.22l1.92-3.32c.12-.22.07-.47-.12-.61l-2.01-1.58zM12 15.6c-1.98 0-3.6-1.62-3.6-3.6s1.62-3.6 3.6-3.6 3.6 1.62 3.6 3.6-1.62 3.6-3.6 3.6z" },
    };

    [ObservableProperty]
    private NavItem? _selectedNavItem;

    partial void OnSelectedNavItemChanged(NavItem? value)
    {
        if (value == null) return;
        if (value.Label == "Dashboard")
        {
            var vm = App.Services.GetRequiredService<DashboardViewModel>();
            _navigationService.NavigateTo(vm);
        }
        else if (value.Label == "Schedule")
        {
            var vm = App.Services.GetRequiredService<ScheduleViewModel>();
            _navigationService.NavigateTo(vm);
        }
        else if (value.Label == "Reminders")
        {
            var vm = App.Services.GetRequiredService<RemindersViewModel>();
            _navigationService.NavigateTo(vm);
        }
        else if (value.Label == "History")
        {
            var vm = App.Services.GetRequiredService<HistoryViewModel>();
            _navigationService.NavigateTo(vm);
        }
        else if (value.Label == "Settings")
        {
            var vm = App.Services.GetRequiredService<SettingsViewModel>();
            _navigationService.NavigateTo(vm);
        }
    }

    public ObservableCollection<string> MedicineTypes { get; } = new()
    { "Tablet", "Capsule", "Syrup", "Injection", "Drops", "Inhaler", "Other" };

    public ObservableCollection<string> Frequencies { get; } = new()
    { "Once daily", "Twice daily", "Three times daily", "Every X hours", "Weekly", "Custom schedule" };

    public ObservableCollection<string> MealTimings { get; } = new()
    { "Before Meal", "After Meal", "Any Time" };

    public ObservableCollection<string> DosageUnits { get; } = new()
    { "tablet", "capsule", "ml", "drops", "puff", "mg", "g" };

    public ObservableCollection<string> DaysOfWeek { get; } = new()
    { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

    public MyMedicinesViewModel(
        IAuthService authService,
        IMedicationService medicationService,
        INavigationService navigationService)
    {
        _authService = authService;
        _medicationService = medicationService;
        _navigationService = navigationService;

        SelectedNavItem = NavItems.First(n => n.Label == "My Medicines");

        if (_authService.CurrentUser != null)
        {
            var hour = DateTime.Now.Hour;
            var greeting = hour < 12 ? "Good morning" : hour < 17 ? "Good afternoon" : "Good evening";
            WelcomeMessage = $"{greeting}, {_authService.CurrentUser.FullName}";
        }

        _ = LoadMedicinesAsync();
    }

    public async Task LoadMedicinesAsync()
    {
        if (_authService.CurrentUser == null) return;

        var meds = await _medicationService.GetUserMedicationsAsync(_authService.CurrentUser.Id);
        System.Windows.Application.Current?.Dispatcher.Invoke(() =>
        {
            Medicines = new ObservableCollection<Medication>(meds);
            ApplyFilter();
        });
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    partial void OnActiveFilterChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var query = Medicines.AsEnumerable();

        if (ActiveFilter == "Active")
            query = query.Where(m => !m.IsArchived);
        else if (ActiveFilter == "Archived")
            query = query.Where(m => m.IsArchived);

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.Trim().ToLowerInvariant();
            query = query.Where(m =>
                (m.OfficialName?.ToLowerInvariant().Contains(search) ?? false) ||
                (m.DisplayName?.ToLowerInvariant().Contains(search) ?? false) ||
                (m.MedicineType?.ToLowerInvariant().Contains(search) ?? false));
        }

        FilteredMedicines = new ObservableCollection<Medication>(query);
    }

    [RelayCommand]
    private void SetFilter(string filter)
    {
        ActiveFilter = filter;
    }

    [RelayCommand]
    private void ShowAddMedicine()
    {
        SelectedMedicine = new Medication
        {
            UserId = _authService.CurrentUser?.Id ?? 0,
            MedicineType = "Tablet",
            Frequency = "Once daily",
            MealTiming = "Any Time",
            DosageUnit = "tablet",
            IsOngoing = true,
            IsArchived = false,
            WeekdaySchedule = Enum.GetValues<DayOfWeek>(),
            ReminderTimes = "08:00",
            StartDate = DateTime.Now,
        };
        IsEditing = false;
        IsPopupOpen = true;
    }

    [RelayCommand]
    private void ShowEditMedicine(Medication medication)
    {
        SelectedMedicine = new Medication
        {
            Id = medication.Id,
            UserId = medication.UserId,
            OfficialName = medication.OfficialName,
            DisplayName = medication.DisplayName,
            DosageValue = medication.DosageValue,
            DosageUnit = medication.DosageUnit,
            MedicineType = medication.MedicineType,
            Frequency = medication.Frequency,
            ReminderTimes = medication.ReminderTimes,
            MealTiming = medication.MealTiming,
            StartDate = medication.StartDate,
            EndDate = medication.EndDate,
            IsOngoing = medication.IsOngoing,
            IsArchived = medication.IsArchived,
            RemainingPills = medication.RemainingPills,
            LowStockAlertAt = medication.LowStockAlertAt,
            IntakeInstructions = medication.IntakeInstructions,
            WeekdaySchedule = medication.WeekdaySchedule,
            ScheduledTime = medication.ScheduledTime,
        };
        IsEditing = true;
        IsPopupOpen = true;
    }

    [RelayCommand]
    private void ClosePopup()
    {
        IsPopupOpen = false;
    }

    [RelayCommand]
    private async Task SaveMedicineAsync()
    {
        if (_authService.CurrentUser == null) return;

        SelectedMedicine.UserId = _authService.CurrentUser.Id;

        // Derive ScheduledTime from first reminder time
        if (!string.IsNullOrWhiteSpace(SelectedMedicine.ReminderTimes))
        {
            var firstTime = SelectedMedicine.ReminderTimes.Split(',').FirstOrDefault();
            if (TimeSpan.TryParse(firstTime, out var ts))
                SelectedMedicine.ScheduledTime = ts;
        }

        if (IsEditing)
            await _medicationService.UpdateAsync(SelectedMedicine);
        else
            await _medicationService.AddAsync(SelectedMedicine);

        IsPopupOpen = false;
        await LoadMedicinesAsync();
    }

    [RelayCommand]
    private async Task DeleteMedicineAsync(Medication medication)
    {
        await _medicationService.DeleteAsync(medication.Id);
        await LoadMedicinesAsync();
    }

    [RelayCommand]
    private async Task ToggleArchiveAsync(Medication medication)
    {
        medication.IsArchived = !medication.IsArchived;
        await _medicationService.UpdateAsync(medication);
        await LoadMedicinesAsync();
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        var loginVm = App.Services.GetRequiredService<LoginViewModel>();
        _navigationService.NavigateTo(loginVm);
    }

    [RelayCommand]
    private void NavigateToDashboard()
    {
        var dashboardVm = App.Services.GetRequiredService<DashboardViewModel>();
        _navigationService.NavigateTo(dashboardVm);
    }
}
