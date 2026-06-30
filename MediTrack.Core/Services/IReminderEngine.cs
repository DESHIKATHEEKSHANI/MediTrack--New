using MediTrack.Core.Models;

namespace MediTrack.Core.Services;

public interface IReminderEngine
{
    event EventHandler<ReminderEventArgs>? ReminderTriggered;
    bool IsRunning { get; }
    void Start();
    void Stop();
    void SnoozeReminder(int medicationId, DateTime scheduledTime, int minutes);
}

public class ReminderEventArgs : EventArgs
{
    public Medication Medication { get; }
    public DateTime ScheduledTime { get; }

    public ReminderEventArgs(Medication medication, DateTime scheduledTime)
    {
        Medication = medication;
        ScheduledTime = scheduledTime;
    }
}
