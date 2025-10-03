using SoftMindApi.Entities;

namespace SoftMindApi.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<bool> ValidateCredentialsAsync(string username, string password);

        // App data users
        Task<User?> GetByDeviceIdAsync(string deviceId);
        Task<User> AddAsync(User user);
        Task<bool> SaveChangesAsync();
    }
}
