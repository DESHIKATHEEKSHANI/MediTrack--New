using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediTrack.Core.Models;
using MediTrack.Core.Services;
using System;
using System.Threading.Tasks;

namespace MediTrack.UI.ViewModels;

public partial class AlertViewModel : ObservableObject
{
    private readonly IIntakeLogService _intakeLogService;
    private readonly Action? _closeAction;

    [ObservableProperty]
    private string _medicationName = string.Empty;

    [ObservableProperty]
    private string _dosageInfo = string.Empty;

    [ObservableProperty]
    private int _snoozeMinutes = 10;

    public int UserId { get; set; }
    public int MedicationId { get; set; }
    public DateTime ScheduledTime { get; set; }
    public Action? OnSnooze { get; set; }

    // Parameterless constructor required for WPF type scanning
    public AlertViewModel() { _intakeLogService = null!; }

    public AlertViewModel(IIntakeLogService intakeLogService, Medication medication, DateTime scheduledTime, Action? closeAction = null)
    {
        _intakeLogService = intakeLogService;
        _closeAction = closeAction;
        MedicationName = BuildMedicationName(medication);
        DosageInfo = $"{medication.DosageValue} {medication.DosageUnit}";
        SnoozeMinutes = medication.SnoozeMinutes ?? 10;
        UserId = medication.UserId;
        MedicationId = medication.Id;
        ScheduledTime = scheduledTime;
    }

    private static string BuildMedicationName(Medication med)
    {
        if (!string.IsNullOrWhiteSpace(med.DisplayName) && !string.Equals(med.DisplayName, med.OfficialName, StringComparison.OrdinalIgnoreCase))
            return $"{med.OfficialName} ({med.DisplayName})";
        return med.OfficialName;
    }

    [RelayCommand]
    private async Task TakenAsync()
    {
        await _intakeLogService.LogActionAsync(UserId, MedicationId, ScheduledTime, IntakeStatus.Taken);
        _closeAction?.Invoke();
    }

    [RelayCommand]
    private async Task MissedAsync()
    {
        await _intakeLogService.LogActionAsync(UserId, MedicationId, ScheduledTime, IntakeStatus.Missed);
        _closeAction?.Invoke();
    }

    [RelayCommand]
    private void Snooze()
    {
        OnSnooze?.Invoke();
        _closeAction?.Invoke();
    }
}
