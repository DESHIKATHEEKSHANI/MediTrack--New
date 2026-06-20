using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediTrack.Core.Models;
using MediTrack.Core.Services;
using MediTrack.UI.Models;
using MediTrack.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace MediTrack.UI.ViewModels;

public partial class HistoryViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IIntakeLogService _intakeLogService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<IntakeLog> _logs = new();

    [ObservableProperty]
    private ObservableCollection<IntakeLog> _filteredLogs = new();

    [ObservableProperty]
    private string _activeFilter = "All";

    [ObservableProperty]
    private string _welcomeMessage = string.Empty;

    [ObservableProperty]
    private ObservableCollection<NavItem> _navItems = new()
    {
        new NavItem { Label = "Dashboard", Icon = "M3 13h8V3H3v10zm0 8h8v-6H3v6zm10 0h8V11h-8v10zm0-18v6h8V3h-8z" },
        new NavItem { Label = "My Medicines", Icon = "M6 3h12v18H6V3zm2 2v14h8V5H8zm3 3h2v2h-2V8zm0 4h2v2h-2v-2z" },
        new NavItem { Label = "Schedule", Icon = "M19 3h-1V1h-2v2H8V1H6v2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H5V8h14v11z" },
        new NavItem { Label = "Reminders", Icon = "M18 16v-5c0-3.07-1.64-5.64-4.5-6.32V4c0-.83-.67-1.5-1.5-1.5s-1.5.67-1.5 1.5v.68C7.63 5.36 6 7.92 6 11v5l-2 2v1h16v-1l-2-2zm-5 0h-2v-2h2v2zm0-4h-2V9h2v3z" },
        new NavItem { Label = "History", Icon = "M13 3a9 9 0 0 0-9 9H1l3.89 3.89.07.14L9 12H6c0-3.87 3.13-7 7-7s7 3.13 7 7-3.13 7-7 7c-1.93 0-3.68-.79-4.94-2.06l-1.42 1.42A8.954 8.954 0 0 0 13 21a9 9 0 0 0 0-18z", IsSelected = true },
        new NavItem { Label = "Settings", Icon = "M19.14 12.94c.04-.3.06-.61.06-.94 0-.32-.02-.64-.07-.94l2.03-1.58a.49.49 0 0 0 .12-.61l-1.92-3.32a.488.488 0 0 0-.59-.22l-2.39.96c-.5-.38-1.03-.7-1.62-.94l-.36-2.54a.484.484 0 0 0-.48-.41h-3.84c-.24 0-.43.17-.47.41l-.36 2.54c-.59.24-1.13.57-1.62.94l-2.39-.96a.488.488 0 0 0-.59.22L3.16 8.87c-.12.21-.08.47.12.61l2.03 1.58c-.05.3-.07.63-.07.94s.02.64.07.94l-2.03 1.58a.49.49 0 0 0-.12.61l1.92 3.32c.12.22.37.29.59.22l2.39-.96c.5.38 1.03.7 1.62.94l.36 2.54c.05.24.24.41.48.41h3.84c.24 0 .44-.17.47-.41l.36-2.54c.59-.24 1.13-.56 1.62-.94l2.39.96c.22.08.47 0 .59-.22l1.92-3.32c.12-.22.07-.47-.12-.61l-2.01-1.58zM12 15.6c-1.98 0-3.6-1.62-3.6-3.6s1.62-3.6 3.6-3.6 3.6 1.62 3.6 3.6-1.62 3.6-3.6 3.6z" },
    };

    [ObservableProperty]
    private NavItem? _selectedNavItem;

    partial void OnSelectedNavItemChanged(NavItem? value)
    {
        if (value == null) return;
        NavigateFromLabel(value.Label);
    }

    partial void OnActiveFilterChanged(string value)
    {
        ApplyFilter();
    }

    public HistoryViewModel(
        IAuthService authService,
        IIntakeLogService intakeLogService,
        INavigationService navigationService)
    {
        _authService = authService;
        _intakeLogService = intakeLogService;
        _navigationService = navigationService;

        SelectedNavItem = NavItems.First(n => n.Label == "History");

        if (_authService.CurrentUser != null)
        {
            var hour = DateTime.Now.Hour;
            var greeting = hour < 12 ? "Good morning" : hour < 17 ? "Good afternoon" : "Good evening";
            WelcomeMessage = $"{greeting}, {_authService.CurrentUser.FullName}";
        }

        _ = LoadLogsAsync();
    }

    private async Task LoadLogsAsync()
    {
        if (_authService.CurrentUser == null) return;

        var logs = await _intakeLogService.GetUserLogsAsync(_authService.CurrentUser.Id);
        System.Windows.Application.Current?.Dispatcher.Invoke(() =>
        {
            Logs = new ObservableCollection<IntakeLog>(logs.OrderByDescending(l => l.ScheduledDateTime));
            ApplyFilter();
        });
    }

    private void ApplyFilter()
    {
        var query = Logs.AsEnumerable();
        if (ActiveFilter == "Taken")
            query = query.Where(l => l.Status == IntakeStatus.Taken);
        else if (ActiveFilter == "Skipped")
            query = query.Where(l => l.Status == IntakeStatus.Dismissed);
        else if (ActiveFilter == "Pending")
            query = query.Where(l => l.Status == IntakeStatus.Pending);

        FilteredLogs = new ObservableCollection<IntakeLog>(query);
    }

    [RelayCommand]
    private void SetFilter(string filter)
    {
        ActiveFilter = filter;
    }

    private void NavigateFromLabel(string label)
    {
        if (label == "Dashboard")
            _navigationService.NavigateTo(App.Services.GetRequiredService<DashboardViewModel>());
        else if (label == "My Medicines")
            _navigationService.NavigateTo(App.Services.GetRequiredService<MyMedicinesViewModel>());
        else if (label == "Schedule")
            _navigationService.NavigateTo(App.Services.GetRequiredService<ScheduleViewModel>());
        else if (label == "Reminders")
            _navigationService.NavigateTo(App.Services.GetRequiredService<RemindersViewModel>());
        else return;

        _selectedNavItem = NavItems[4];
        OnPropertyChanged(nameof(SelectedNavItem));
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        var loginVm = App.Services.GetRequiredService<LoginViewModel>();
        _navigationService.NavigateTo(loginVm);
    }
}
