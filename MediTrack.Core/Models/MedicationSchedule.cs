namespace MediTrack.Core.Models;

public class MedicationSchedule
{
    public int Id { get; set; }
    public int MedicationId { get; set; }
    public TimeSpan ReminderTime { get; set; }
    public DayOfWeek[] WeekdaySchedule { get; set; } = Array.Empty<DayOfWeek>();
    public string MealTiming { get; set; } = "Any Time";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsOngoing { get; set; } = true;
    public bool IsActive { get; set; } = true;

    public Medication Medication { get; set; } = null!;
    public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
}
