using SoftMindApi.DTO;

namespace SoftMindApi.Services.Interface
{
    public interface IAlertService
    {
        Task<AlertDTO?> GetRandomAlertAsync(string deviceId);
        Task<List<AlertDTO>> GetRecentAlertsAsync(string deviceId, int take = 50);
        Task<bool> MarkAsReadAsync(string deviceId, string alertId);
        Task<AlertDTO> CreateAlertAsync(string deviceId, CreateAlertDTO dto);
    }
}
