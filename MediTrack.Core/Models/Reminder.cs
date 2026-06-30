namespace MediTrack.Core.Models;

public enum ReminderStatus
{
    Taken,
    Pending,
    Missed,
    Snoozed
}

public class Reminder
{
    public int Id { get; set; }
    public int MedicationId { get; set; }
    public int ScheduleId { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public ReminderStatus Status { get; set; } = ReminderStatus.Pending;
    public DateTime? SnoozeUntil { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Medication Medication { get; set; } = null!;
    public MedicationSchedule Schedule { get; set; } = null!;
    public ICollection<IntakeLog> IntakeLogs { get; set; } = new List<IntakeLog>();
}
