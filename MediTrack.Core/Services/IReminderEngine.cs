using MediTrack.Core.Models;

namespace MediTrack.Core.Services;

public interface IReminderEngine
{
    event EventHandler<ReminderEventArgs>? ReminderTriggered;
    void Start();
    void Stop();
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
