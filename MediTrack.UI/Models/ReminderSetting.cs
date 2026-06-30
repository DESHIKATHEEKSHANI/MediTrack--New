namespace MediTrack.UI.Models;

public class ReminderSetting
{
    public int Id { get; set; }
    public int? MedicationId { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public string Time { get; set; } = "08:00";
    public string Frequency { get; set; } = "Daily";
    public int SnoozeMinutes { get; set; } = 10;
    public string ActiveDays { get; set; } = "Every day";
}
