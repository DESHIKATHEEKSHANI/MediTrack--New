using MediTrack.Core.Models;

namespace MediTrack.UI.Models;

public class ScheduleItem
{
    public int MedicationId { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public TimeSpan ScheduledTime { get; set; }
    public DateTime Date { get; set; }
    public DateTime ScheduledDateTime => Date.Date + ScheduledTime;
    public string Dosage { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public IntakeStatus Status { get; set; }
    public bool IsTaken => Status == IntakeStatus.Taken;
    public bool IsPending => Status == IntakeStatus.Pending && !IsMissed;
    public bool IsMissed => Status == IntakeStatus.Missed;
    public string StatusText => Status.ToString();
}
