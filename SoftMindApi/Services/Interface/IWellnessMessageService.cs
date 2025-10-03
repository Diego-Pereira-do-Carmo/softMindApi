using SoftMindApi.DTO;

namespace SoftMindApi.Services.Interface
{
    public interface IWellnessMessageService
    {
        Task<List<WellnessMessageDTO>> GetRandomForDeviceAsync(string deviceId, int take = 5);
    }
}
