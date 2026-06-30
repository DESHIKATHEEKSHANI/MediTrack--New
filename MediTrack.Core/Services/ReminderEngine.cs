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
    private readonly List<SnoozedReminder> _snoozed = new();
    private int? _currentUserId;
    private bool _isRunning;

    public event EventHandler<ReminderEventArgs>? ReminderTriggered;
    public bool IsRunning => _isRunning;

    public ReminderEngine(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _timer = new Timer(30000); // check every 30 seconds
        _timer.Elapsed += OnTimerElapsed;
    }

    public void SetUser(int userId) => _currentUserId = userId;

    public void Start()
    {
        _isRunning = true;
        _timer.Start();
    }

    public void Stop()
    {
        _isRunning = false;
        _timer.Stop();
    }

    public void SnoozeReminder(int medicationId, DateTime scheduledTime, int minutes)
    {
        lock (_snoozed)
        {
            _snoozed.RemoveAll(s => s.MedicationId == medicationId && s.ScheduledTime == scheduledTime);
            _snoozed.Add(new SnoozedReminder(medicationId, scheduledTime, DateTime.Now.AddMinutes(minutes)));
        }
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

            // Check snoozed reminders
            List<SnoozedReminder> dueSnoozed;
            lock (_snoozed)
            {
                dueSnoozed = _snoozed.Where(s => s.SnoozeUntil <= now).ToList();
                _snoozed.RemoveAll(s => s.SnoozeUntil <= now);
            }

            var medications = await context.Medications
                .AsNoTracking()
                .Include(m => m.Schedules)
                .Include(m => m.Snooze)
                .Where(m => m.UserId == _currentUserId && m.IsActive)
                .ToListAsync();

            foreach (var med in medications)
            {
                foreach (var schedule in med.Schedules.Where(s => s.IsActive && s.WeekdaySchedule.Contains(now.DayOfWeek)))
                {
                    var scheduledToday = now.Date.Add(schedule.ReminderTime);
                    if (scheduledToday < windowStart || scheduledToday > windowEnd)
                        continue;

                    if (schedule.EndDate.HasValue && scheduledToday > schedule.EndDate.Value) continue;
                    if (schedule.StartDate.HasValue && scheduledToday < schedule.StartDate.Value) continue;

                    var alreadyLogged = await context.Reminders.AnyAsync(r =>
                        r.MedicationId == med.Id &&
                        r.ScheduleId == schedule.Id &&
                        r.ScheduledDateTime == scheduledToday &&
                        r.Status != ReminderStatus.Pending);

                    if (!alreadyLogged)
                        ReminderTriggered?.Invoke(this, new ReminderEventArgs(med, scheduledToday));
                }

                // Re-trigger snoozed reminders
                foreach (var snooze in dueSnoozed.Where(s => s.MedicationId == med.Id))
                {
                    var alreadyLogged = await context.Reminders.AnyAsync(r =>
                        r.MedicationId == med.Id &&
                        r.ScheduledDateTime == snooze.ScheduledTime &&
                        r.Status != ReminderStatus.Pending);

                    if (!alreadyLogged)
                        ReminderTriggered?.Invoke(this, new ReminderEventArgs(med, snooze.ScheduledTime));
                }
            }
        }
        catch { /* swallow background errors */ }
    }

    private record SnoozedReminder(int MedicationId, DateTime ScheduledTime, DateTime SnoozeUntil);
}
