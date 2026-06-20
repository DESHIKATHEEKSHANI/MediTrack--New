namespace MediTrack.Core.Models;

public enum IntakeStatus
{
    Taken,
    Pending,
    Dismissed
}

public class IntakeLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int MedicationId { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public DateTime? ActionTimestamp { get; set; }
    public IntakeStatus Status { get; set; }
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Medication Medication { get; set; } = null!;
}
