using MediTrack.Core.Data;
using MediTrack.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace MediTrack.Core.Services;

public class IntakeLogService : IIntakeLogService
{
    private readonly MediTrackDbContext _context;

    public event EventHandler<Medication>? LowStockAlertTriggered;

    public IntakeLogService(MediTrackDbContext context)
    {
        _context = context;
    }

    public async Task<IntakeLog> LogActionAsync(int userId, int medicationId, DateTime scheduledDateTime, IntakeStatus status)
    {
        // Try to find an existing reminder for this dose
        var reminder = await _context.Reminders
            .FirstOrDefaultAsync(r => r.MedicationId == medicationId && r.ScheduledDateTime == scheduledDateTime);

        if (reminder == null)
        {
            // No pre-generated reminder found — look up the matching schedule so the FK is valid
            var timeOfDay = scheduledDateTime.TimeOfDay;
            var scheduleId = await _context.MedicationSchedules
                .Where(s => s.MedicationId == medicationId && s.IsActive && s.ReminderTime == timeOfDay)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            // Fallback: any active schedule for this medication
            if (scheduleId == 0)
                scheduleId = await _context.MedicationSchedules
                    .Where(s => s.MedicationId == medicationId && s.IsActive)
                    .Select(s => s.Id)
                    .FirstOrDefaultAsync();

            if (scheduleId == 0)
            {
                // No valid schedule exists — create a placeholder IntakeLog without a Reminder row
                var fallback = new IntakeLog
                {
                    ReminderId = 0,
                    UserId = userId,
                    MedicationId = medicationId,
                    ActionTimestamp = DateTime.UtcNow,
                    Status = status
                };
                return fallback;
            }

            reminder = new Reminder
            {
                MedicationId = medicationId,
                ScheduleId = scheduleId,
                ScheduledDateTime = scheduledDateTime,
                Status = status == IntakeStatus.Taken ? ReminderStatus.Taken : ReminderStatus.Missed
            };
            _context.Reminders.Add(reminder);
            await _context.SaveChangesAsync();
        }
        else
        {
            reminder.Status = status == IntakeStatus.Taken ? ReminderStatus.Taken : ReminderStatus.Missed;
        }

        var existing = await _context.IntakeLogs
            .FirstOrDefaultAsync(l => l.ReminderId == reminder.Id);
        if (existing != null)
        {
            existing.Status = status;
            existing.ActionTimestamp = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            await DecrementPillsAndCheckLowStockAsync(medicationId, status);
            return existing;
        }

        var log = new IntakeLog
        {
            ReminderId = reminder.Id,
            UserId = userId,
            MedicationId = medicationId,
            ActionTimestamp = DateTime.UtcNow,
            Status = status
        };
        _context.IntakeLogs.Add(log);
        await _context.SaveChangesAsync();
        await DecrementPillsAndCheckLowStockAsync(medicationId, status);
        return log;
    }

    private async Task DecrementPillsAndCheckLowStockAsync(int medicationId, IntakeStatus status)
    {
        if (status != IntakeStatus.Taken) return;

        var med = await _context.Medications
            .Include(m => m.Inventory)
            .FirstOrDefaultAsync(m => m.Id == medicationId);
        if (med == null || med.Inventory == null || !med.Inventory.RemainingPills.HasValue) return;

        med.Inventory.RemainingPills = Math.Max(0, med.Inventory.RemainingPills.Value - 1);
        await _context.SaveChangesAsync();

        if (med.Inventory.LowStockAlertAt.HasValue && med.Inventory.RemainingPills.Value <= med.Inventory.LowStockAlertAt.Value)
        {
            LowStockAlertTriggered?.Invoke(this, med);
        }
    }

    public async Task<IEnumerable<IntakeLog>> GetUserLogsAsync(int userId, DateTime? from = null, DateTime? to = null)
    {
        var query = _context.IntakeLogs
            .AsNoTracking()
            .Include(l => l.Medication)
            .Include(l => l.Reminder)
            .Where(l => l.UserId == userId);
        if (from.HasValue)
            query = query.Where(l => l.Reminder != null && l.Reminder.ScheduledDateTime >= from.Value);
        if (to.HasValue)
            query = query.Where(l => l.Reminder != null && l.Reminder.ScheduledDateTime <= to.Value);

        return await query.OrderByDescending(l => l.Reminder!.ScheduledDateTime).ToListAsync();
    }

    public async Task<IEnumerable<IntakeLog>> GetTodayLogsAsync(int userId)
    {
        var today = DateTime.Now.Date;
        var tomorrow = today.AddDays(1);
        return await _context.IntakeLogs
            .AsNoTracking()
            .Include(l => l.Medication)
            .Include(l => l.Reminder)
            .Where(l => l.UserId == userId && l.Reminder != null && l.Reminder.ScheduledDateTime >= today && l.Reminder.ScheduledDateTime < tomorrow)
            .OrderBy(l => l.Reminder!.ScheduledDateTime)
            .ToListAsync();
    }

    public async Task<double> GetWeeklyComplianceAsync(int userId)
    {
        var weekAgo = DateTime.Now.AddDays(-7);
        var logs = await _context.IntakeLogs
            .AsNoTracking()
            .Include(l => l.Reminder)
            .Where(l => l.UserId == userId && l.Reminder != null && l.Reminder.ScheduledDateTime >= weekAgo)
            .ToListAsync();

        if (logs.Count == 0) return 0;

        var taken = logs.Count(l => l.Status == IntakeStatus.Taken);
        return (double)taken / logs.Count * 100.0;
    }

    public async Task<List<double>> GetWeeklyAdherenceAsync(int userId)
    {
        var result = new List<double>();
        var today = DateTime.Now.Date;

        for (int i = 6; i >= 0; i--)
        {
            var day = today.AddDays(-i);
            var nextDay = day.AddDays(1);
            var logs = await _context.IntakeLogs
                .AsNoTracking()
                .Include(l => l.Reminder)
                .Where(l => l.UserId == userId && l.Reminder != null && l.Reminder.ScheduledDateTime >= day && l.Reminder.ScheduledDateTime < nextDay)
                .ToListAsync();

            if (logs.Count == 0)
            {
                result.Add(0);
            }
            else
            {
                var taken = logs.Count(l => l.Status == IntakeStatus.Taken);
                result.Add((double)taken / logs.Count * 100.0);
            }
        }

        return result;
    }

    public async Task GenerateScheduledLogsAsync(int userId)
    {
        var medications = await _context.Medications
            .Include(m => m.Schedules)
            .Where(m => m.UserId == userId && m.IsActive)
            .ToListAsync();

        var today = DateTime.Now;
        var todayStart = today.Date;

        foreach (var med in medications)
        {
            foreach (var schedule in med.Schedules.Where(s => s.IsActive && s.WeekdaySchedule.Contains(today.DayOfWeek)))
            {
                var scheduled = todayStart.Add(schedule.ReminderTime);

                if (schedule.EndDate.HasValue && scheduled > schedule.EndDate.Value) continue;
                if (schedule.StartDate.HasValue && scheduled < schedule.StartDate.Value) continue;

                var reminder = await _context.Reminders
                    .FirstOrDefaultAsync(r => r.MedicationId == med.Id && r.ScheduleId == schedule.Id && r.ScheduledDateTime == scheduled);

                if (reminder == null)
                {
                    reminder = new Reminder
                    {
                        MedicationId = med.Id,
                        ScheduleId = schedule.Id,
                        ScheduledDateTime = scheduled,
                        Status = ReminderStatus.Pending
                    };
                    _context.Reminders.Add(reminder);
                    await _context.SaveChangesAsync();
                }

                var exists = await _context.IntakeLogs.AnyAsync(l => l.ReminderId == reminder.Id);
                if (!exists)
                {
                    _context.IntakeLogs.Add(new IntakeLog
                    {
                        ReminderId = reminder.Id,
                        UserId = userId,
                        MedicationId = med.Id,
                        Status = IntakeStatus.Pending
                    });
                }
            }
        }

        await _context.SaveChangesAsync();
    }
}
