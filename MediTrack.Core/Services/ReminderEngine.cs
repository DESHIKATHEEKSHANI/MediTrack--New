using System.Timers;
using MediTrack.Core.Data;
using MediTrack.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Timer = System.Timers.Timer;

namespace MediTrack.Core.Services;

public class ReminderEngine : IReminderEngine
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Timer _timer;
    private int? _currentUserId;

    public event EventHandler<ReminderEventArgs>? ReminderTriggered;

    public ReminderEngine(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _timer = new Timer(30000); // check every 30 seconds
        _timer.Elapsed += OnTimerElapsed;
    }

    public void SetUser(int userId) => _currentUserId = userId;

    public void Start()
    {
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }

    private async void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_currentUserId == null) return;

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MediTrackDbContext>();
            var now = DateTime.Now;
            var windowStart = now.AddMinutes(-2);
            var windowEnd = now.AddMinutes(2);

            var medications = await context.Medications
                .AsNoTracking()
                .Where(m => m.UserId == _currentUserId && m.IsActive)
                .ToListAsync();

            foreach (var med in medications)
            {
                if (!med.WeekdaySchedule.Contains(now.DayOfWeek))
                    continue;

                var scheduledToday = now.Date.Add(med.ScheduledTime);
                if (scheduledToday >= windowStart && scheduledToday <= windowEnd)
                {
                    var alreadyLogged = await context.IntakeLogs.AnyAsync(l =>
                        l.MedicationId == med.Id &&
                        l.ScheduledDateTime == scheduledToday &&
                        l.Status != IntakeStatus.Pending);

                    if (!alreadyLogged)
                    {
                        ReminderTriggered?.Invoke(this, new ReminderEventArgs(med, scheduledToday));
                    }
                }
            }
        }
        catch { /* swallow background errors */ }
    }
}
