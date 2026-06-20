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
        return await _context.Medications
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Medication>> GetUserMedicationsAsync(int userId)
    {
        return await _context.Medications
            .AsNoTracking()
            .Where(m => m.UserId == userId && m.IsActive)
            .ToListAsync();
    }

    public async Task<Medication> AddAsync(Medication medication)
    {
        _context.Medications.Add(medication);
        await _context.SaveChangesAsync();
        return medication;
    }

    public async Task<Medication?> UpdateAsync(Medication medication)
    {
        var existing = await _context.Medications.FindAsync(medication.Id);
        if (existing == null) return null;

        existing.OfficialName = medication.OfficialName;
        existing.DisplayName = medication.DisplayName;
        existing.DosageValue = medication.DosageValue;
        existing.DosageUnit = medication.DosageUnit;
        existing.IntakeInstructions = medication.IntakeInstructions;
        existing.WeekdaySchedule = medication.WeekdaySchedule;
        existing.ScheduledTime = medication.ScheduledTime;
        existing.MedicineType = medication.MedicineType;
        existing.Frequency = medication.Frequency;
        existing.ReminderTimes = medication.ReminderTimes;
        existing.MealTiming = medication.MealTiming;
        existing.StartDate = medication.StartDate;
        existing.EndDate = medication.EndDate;
        existing.IsOngoing = medication.IsOngoing;
        existing.IsArchived = medication.IsArchived;
        existing.RemainingPills = medication.RemainingPills;
        existing.LowStockAlertAt = medication.LowStockAlertAt;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
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
            .Where(m => m.UserId == userId && m.IsActive)
            .ToListAsync();
        return meds.Where(m => m.WeekdaySchedule.Contains(today));
    }
}
