using MediTrack.Core.Models;

namespace MediTrack.Core.Services;

public interface IIntakeLogService
{
    event EventHandler<Medication>? LowStockAlertTriggered;
    Task<IntakeLog> LogActionAsync(int userId, int medicationId, DateTime scheduledDateTime, IntakeStatus status);
    Task<IEnumerable<IntakeLog>> GetUserLogsAsync(int userId, DateTime? from = null, DateTime? to = null);
    Task<IEnumerable<IntakeLog>> GetTodayLogsAsync(int userId);
    Task<double> GetWeeklyComplianceAsync(int userId);
    Task<List<double>> GetWeeklyAdherenceAsync(int userId);
    Task GenerateScheduledLogsAsync(int userId);
}
