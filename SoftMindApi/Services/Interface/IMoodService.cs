using SoftMindApi.Entities;

namespace SoftMindApi.Services.Interface
{
    public interface IMoodService
    {
        Task<List<Mood>> GetMoodLastSevenDaysAsync(string deviceId);
        Task<Mood> AddMoodAsync(string deviceId, string emojiName);
    }
}
