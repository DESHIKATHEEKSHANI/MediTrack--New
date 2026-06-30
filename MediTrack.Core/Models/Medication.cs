using System.ComponentModel.DataAnnotations.Schema;

namespace MediTrack.Core.Models;

public class Medication
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string OfficialName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public double DosageValue { get; set; }
    public string DosageUnit { get; set; } = string.Empty;
    public string MedicineType { get; set; } = "Tablet";
    public string IntakeInstructions { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsArchived { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<MedicationSchedule> Schedules { get; set; } = new List<MedicationSchedule>();
    public MedicationInventory? Inventory { get; set; }
    public MedicationSnooze? Snooze { get; set; }
    public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    public ICollection<IntakeLog> IntakeLogs { get; set; } = new List<IntakeLog>();

    // NotMapped: UI backward-compat during transition
    [NotMapped]
    public string Frequency { get; set; } = "Once daily";
    [NotMapped]
    public string ReminderTimes { get; set; } = string.Empty;
    [NotMapped]
    public string MealTiming { get; set; } = "Any Time";
    [NotMapped]
    public DateTime? StartDate { get; set; }
    [NotMapped]
    public DateTime? EndDate { get; set; }
    [NotMapped]
    public bool IsOngoing { get; set; } = true;
    [NotMapped]
    public int? RemainingPills { get; set; }
    [NotMapped]
    public int? LowStockAlertAt { get; set; }
    [NotMapped]
    public int? SnoozeMinutes { get; set; }
    [NotMapped]
    public DayOfWeek[] WeekdaySchedule { get; set; } = Array.Empty<DayOfWeek>();
    [NotMapped]
    public TimeSpan ScheduledTime { get; set; }
}
