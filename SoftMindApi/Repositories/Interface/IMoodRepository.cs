using SoftMindApi.Entities;

namespace SoftMindApi.Repositories.Interface
{
    public interface IMoodRepository
    {
        Task<List<Mood>> GetLastSevenDaysAsync(string deviceId);
        Task AddAsync(Mood mood);
        Task<bool> SaveChangesAsync();
    }
}
