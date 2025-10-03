using SoftMindApi.Entities;

namespace SoftMindApi.Repositories.Interface
{
    public interface IAlertRepository
    {
        Task<List<string>> GetUnreadMessagesByDeviceAsync(string deviceId);
        Task<List<Alert>> GetRecentAlertsByDeviceAsync(string deviceId, int take = 50);
        Task<Alert?> GetByIdForDeviceAsync(string alertId, string deviceId);
        Task<Alert> AddAsync(Alert alert);
        Task<bool> SaveChangesAsync();
    }
}
