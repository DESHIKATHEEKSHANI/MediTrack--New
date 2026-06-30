using MediTrack.Core.Data;
using MediTrack.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace MediTrack.Core.Services;

public class MedicationService : IMedicationService
{
    private readonly MediTrackDbContext _context;

    public MedicationService(MediTrackDbContext context)
    {
        _context = context;
    }

    public async Task<Medication?> GetByIdAsync(int id)
    {
        var med = await _context.Medications
            .AsNoTracking()
            .Include(m => m.Schedules)
            .Include(m => m.Inventory)
            .Include(m => m.Snooze)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (med != null) HydrateFlatProperties(med);
        return med;
    }

    public async Task<IEnumerable<Medication>> GetUserMedicationsAsync(int userId)
    {
        var meds = await _context.Medications
            .AsNoTracking()
            .Include(m => m.Schedules)
            .Include(m => m.Inventory)
            .Include(m => m.Snooze)
            .Where(m => m.UserId == userId && m.IsActive)
            .ToListAsync();
        foreach (var med in meds) HydrateFlatProperties(med);
        return meds;
    }

    public async Task<Medication> AddAsync(Medication medication)
    {
        SyncRelatedEntities(medication);
        _context.Medications.Add(medication);
        await _context.SaveChangesAsync();
        return medication;
    }

    public async Task<Medication?> UpdateAsync(Medication medication)
    {
        var existing = await _context.Medications
            .Include(m => m.Schedules)
            .Include(m => m.Inventory)
            .Include(m => m.Snooze)
            .FirstOrDefaultAsync(m => m.Id == medication.Id);
        if (existing == null) return null;

        // Update scalar fields on the tracked entity
        existing.OfficialName = medication.OfficialName;
        existing.DisplayName = medication.DisplayName;
        existing.DosageValue = medication.DosageValue;
        existing.DosageUnit = medication.DosageUnit;
        existing.MedicineType = medication.MedicineType;
        existing.IntakeInstructions = medication.IntakeInstructions;
        existing.IsArchived = medication.IsArchived;
        existing.UpdatedAt = DateTime.UtcNow;

        // Sync schedules directly on the EF-tracked collection so SaveChanges picks up the changes
        existing.Schedules.Clear();
        var times = GetReminderTimes(medication).Distinct().ToList();
        var days = medication.WeekdaySchedule?.Any() == true
            ? medication.WeekdaySchedule
            : Enum.GetValues<DayOfWeek>();
        if (!times.Any() && medication.ScheduledTime != default)
            times.Add(medication.ScheduledTime);
        foreach (var ts in times)
        {
            existing.Schedules.Add(new MedicationSchedule
            {
                MedicationId = existing.Id,
                ReminderTime = ts,
                WeekdaySchedule = days,
                MealTiming = medication.MealTiming,
                StartDate = medication.StartDate,
                EndDate = medication.EndDate,
                IsOngoing = medication.IsOngoing,
                IsActive = true
            });
        }

        // Sync inventory directly on the tracked reference
        if (medication.RemainingPills.HasValue || medication.LowStockAlertAt.HasValue)
        {
            existing.Inventory ??= new MedicationInventory { MedicationId = existing.Id };
            existing.Inventory.RemainingPills = medication.RemainingPills;
            existing.Inventory.LowStockAlertAt = medication.LowStockAlertAt;
        }

        // Sync snooze directly on the tracked reference
        if (medication.SnoozeMinutes.HasValue)
        {
            existing.Snooze ??= new MedicationSnooze { MedicationId = existing.Id };
            existing.Snooze.SnoozeMinutes = medication.SnoozeMinutes.Value;
        }

        await _context.SaveChangesAsync();
        HydrateFlatProperties(existing);
        return existing;
    }

    private static void HydrateFlatProperties(Medication med)
    {
        var schedules = med.Schedules.Where(s => s.IsActive).ToList();
        med.WeekdaySchedule = schedules.SelectMany(s => s.WeekdaySchedule).Distinct().ToArray();
        med.ScheduledTime = schedules.FirstOrDefault()?.ReminderTime ?? default;
        med.ReminderTimes = string.Join(",", schedules.Select(s => s.ReminderTime.ToString("hh\\:mm")));
        med.Frequency = schedules.Count == 1 ? "Once daily" : schedules.Count == 2 ? "Twice daily" : schedules.Count == 3 ? "Three times daily" : schedules.Count > 0 ? "Custom" : "Once daily";
        med.MealTiming = schedules.FirstOrDefault()?.MealTiming ?? "Any Time";
        med.StartDate = schedules.FirstOrDefault()?.StartDate;
        med.EndDate = schedules.FirstOrDefault()?.EndDate;
        med.IsOngoing = schedules.FirstOrDefault()?.IsOngoing ?? true;
        med.RemainingPills = med.Inventory?.RemainingPills;
        med.LowStockAlertAt = med.Inventory?.LowStockAlertAt;
        med.SnoozeMinutes = med.Snooze?.SnoozeMinutes;
    }

    private static void SyncRelatedEntities(Medication med)
    {
        // Schedules
        var times = GetReminderTimes(med).Distinct().ToList();
        var days = med.WeekdaySchedule?.Any() == true ? med.WeekdaySchedule : Enum.GetValues<DayOfWeek>();
        med.Schedules.Clear();
        foreach (var ts in times)
        {
            med.Schedules.Add(new MedicationSchedule
            {
                MedicationId = med.Id,
                ReminderTime = ts,
                WeekdaySchedule = days,
                MealTiming = med.MealTiming,
                StartDate = med.StartDate,
                EndDate = med.EndDate,
                IsOngoing = med.IsOngoing,
                IsActive = true
            });
        }
        if (!times.Any() && med.ScheduledTime != default)
        {
            med.Schedules.Add(new MedicationSchedule
            {
                MedicationId = med.Id,
                ReminderTime = med.ScheduledTime,
                WeekdaySchedule = days,
                MealTiming = med.MealTiming,
                StartDate = med.StartDate,
                EndDate = med.EndDate,
                IsOngoing = med.IsOngoing,
                IsActive = true
            });
        }

        // Inventory
        if (med.RemainingPills.HasValue || med.LowStockAlertAt.HasValue)
        {
            med.Inventory ??= new MedicationInventory { MedicationId = med.Id };
            med.Inventory.RemainingPills = med.RemainingPills;
            med.Inventory.LowStockAlertAt = med.LowStockAlertAt;
        }

        // Snooze
        if (med.SnoozeMinutes.HasValue)
        {
            med.Snooze ??= new MedicationSnooze { MedicationId = med.Id };
            med.Snooze.SnoozeMinutes = med.SnoozeMinutes.Value;
        }
    }

    private static IEnumerable<TimeSpan> GetReminderTimes(Medication med)
    {
        var found = false;
        if (!string.IsNullOrWhiteSpace(med.ReminderTimes))
        {
            foreach (var part in med.ReminderTimes.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (TimeSpan.TryParse(part.Trim(), out var ts))
                {
                    found = true;
                    yield return ts;
                }
            }
        }
        if (!found && med.ScheduledTime != default)
            yield return med.ScheduledTime;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var medication = await _context.Medications.FindAsync(id);
        if (medication == null) return false;

        medication.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Medication>> GetTodayScheduleAsync(int userId)
    {
        var today = DateTime.Now.DayOfWeek;
        var meds = await _context.Medications
            .AsNoTracking()
            .Include(m => m.Schedules)
            .Where(m => m.UserId == userId && m.IsActive)
            .ToListAsync();
        return meds.Where(m => m.Schedules.Any(s => s.WeekdaySchedule.Contains(today)));
    }
}
