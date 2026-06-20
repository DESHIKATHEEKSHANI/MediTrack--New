namespace MediTrack.Core.Models;

public class Medication
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string OfficialName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public double DosageValue { get; set; }
    public string DosageUnit { get; set; } = string.Empty;
    public string IntakeInstructions { get; set; } = string.Empty;
    public DayOfWeek[] WeekdaySchedule { get; set; } = Array.Empty<DayOfWeek>();
    public TimeSpan ScheduledTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // New profile fields
    public string MedicineType { get; set; } = "Tablet";
    public string Frequency { get; set; } = "Once daily";
    public string ReminderTimes { get; set; } = string.Empty;
    public string MealTiming { get; set; } = "Any Time";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsOngoing { get; set; } = true;
    public bool IsArchived { get; set; } = false;
    public int? RemainingPills { get; set; }
    public int? LowStockAlertAt { get; set; }

    public User User { get; set; } = null!;
    public ICollection<IntakeLog> IntakeLogs { get; set; } = new List<IntakeLog>();
}
