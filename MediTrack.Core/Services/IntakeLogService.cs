using MediTrack.Core.Data;
using MediTrack.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace MediTrack.Core.Services;

public class IntakeLogService : IIntakeLogService
{
    private readonly MediTrackDbContext _context;

    public IntakeLogService(MediTrackDbContext context)
    {
        _context = context;
    }

    public async Task<IntakeLog> LogActionAsync(int userId, int medicationId, DateTime scheduledDateTime, IntakeStatus status)
    {
        var existing = await _context.IntakeLogs
            .FirstOrDefaultAsync(l => l.UserId == userId && l.MedicationId == medicationId && l.ScheduledDateTime == scheduledDateTime);
        if (existing != null)
        {
            existing.Status = status;
            existing.ActionTimestamp = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existing;
        }

        var log = new IntakeLog
        {
            UserId = userId,
            MedicationId = medicationId,
            ScheduledDateTime = scheduledDateTime,
            ActionTimestamp = DateTime.UtcNow,
            Status = status
        };
        _context.IntakeLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<IEnumerable<IntakeLog>> GetUserLogsAsync(int userId, DateTime? from = null, DateTime? to = null)
    {
        var query = _context.IntakeLogs.AsNoTracking().Include(l => l.Medication).Where(l => l.UserId == userId);
        if (from.HasValue)
            query = query.Where(l => l.ScheduledDateTime >= from.Value);
        if (to.HasValue)
            query = query.Where(l => l.ScheduledDateTime <= to.Value);

        return await query.OrderByDescending(l => l.ScheduledDateTime).ToListAsync();
    }

    public async Task<IEnumerable<IntakeLog>> GetTodayLogsAsync(int userId)
    {
        var today = DateTime.Now.Date;
        var tomorrow = today.AddDays(1);
        return await _context.IntakeLogs
            .AsNoTracking()
            .Include(l => l.Medication)
            .Where(l => l.UserId == userId && l.ScheduledDateTime >= today && l.ScheduledDateTime < tomorrow)
            .OrderBy(l => l.ScheduledDateTime)
            .ToListAsync();
    }

    public async Task<double> GetWeeklyComplianceAsync(int userId)
    {
        var weekAgo = DateTime.Now.AddDays(-7);
        var logs = await _context.IntakeLogs
            .AsNoTracking()
            .Where(l => l.UserId == userId && l.ScheduledDateTime >= weekAgo)
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
                .Where(l => l.UserId == userId && l.ScheduledDateTime >= day && l.ScheduledDateTime < nextDay)
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
            .Where(m => m.UserId == userId && m.IsActive)
            .ToListAsync();

        var today = DateTime.Now;
        var todayStart = today.Date;

        foreach (var med in medications)
        {
            if (!med.WeekdaySchedule.Contains(today.DayOfWeek))
                continue;

            var scheduled = todayStart.Add(med.ScheduledTime);
            var exists = await _context.IntakeLogs.AnyAsync(l =>
                l.MedicationId == med.Id &&
                l.ScheduledDateTime == scheduled);

            if (!exists)
            {
                _context.IntakeLogs.Add(new IntakeLog
                {
                    UserId = userId,
                    MedicationId = med.Id,
                    ScheduledDateTime = scheduled,
                    Status = IntakeStatus.Pending
                });
            }
        }

        await _context.SaveChangesAsync();
    }
}
