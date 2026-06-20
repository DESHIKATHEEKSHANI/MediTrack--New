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

    public int UserId { get; set; }
    public int MedicationId { get; set; }
    public DateTime ScheduledTime { get; set; }

    public AlertViewModel(IIntakeLogService intakeLogService, Medication medication, DateTime scheduledTime, Action? closeAction = null)
    {
        _intakeLogService = intakeLogService;
        _closeAction = closeAction;
        MedicationName = medication.DisplayName ?? medication.OfficialName;
        DosageInfo = $"{medication.DosageValue} {medication.DosageUnit}";
        UserId = medication.UserId;
        MedicationId = medication.Id;
        ScheduledTime = scheduledTime;
    }

    [RelayCommand]
    private async Task TakenAsync()
    {
        await _intakeLogService.LogActionAsync(UserId, MedicationId, ScheduledTime, IntakeStatus.Taken);
        _closeAction?.Invoke();
    }

    [RelayCommand]
    private async Task DismissAsync()
    {
        await _intakeLogService.LogActionAsync(UserId, MedicationId, ScheduledTime, IntakeStatus.Dismissed);
        _closeAction?.Invoke();
    }
}
