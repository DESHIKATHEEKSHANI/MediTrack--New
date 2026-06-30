namespace MediTrack.Core.Models;

public class MedicationInventory
{
    public int Id { get; set; }
    public int MedicationId { get; set; }
    public int? RemainingPills { get; set; }
    public int? LowStockAlertAt { get; set; }

    public Medication Medication { get; set; } = null!;
}
