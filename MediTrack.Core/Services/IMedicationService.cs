using MediTrack.Core.Models;

namespace MediTrack.Core.Services;

public interface IMedicationService
{
    Task<Medication?> GetByIdAsync(int id);
    Task<IEnumerable<Medication>> GetUserMedicationsAsync(int userId);
    Task<Medication> AddAsync(Medication medication);
    Task<Medication?> UpdateAsync(Medication medication);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Medication>> GetTodayScheduleAsync(int userId);
}
