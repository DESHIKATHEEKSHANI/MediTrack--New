namespace MediTrack.Core.Models;

public class MedicationSnooze
{
    public int Id { get; set; }
    public int MedicationId { get; set; }
    public int SnoozeMinutes { get; set; } = 5;

    public Medication Medication { get; set; } = null!;
}
