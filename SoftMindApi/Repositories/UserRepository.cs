using Microsoft.EntityFrameworkCore;
using SoftMindApi.Configuration;
using SoftMindApi.Data;
using SoftMindApi.Entities;
using SoftMindApi.Repositories.Interface;

namespace SoftMindApi.Repositories
{
    // Current implementation validates against ApiCredentials from configuration.
    // Can be replaced later with a DB-backed user storage.
    public class UserRepository : IUserRepository
    {
        private readonly MongoDbContext _context;
        private readonly ApiCredentials _credentials;

        public UserRepository(MongoDbContext context, ApiCredentials credentials)
        {
            _context = context;
            _credentials = credentials;
        }

        public Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var valid = username == _credentials.Username && password == _credentials.Password;
            return Task.FromResult(valid);
        }

        public async Task<User?> GetByDeviceIdAsync(string deviceId)
        {
            return await _context.User.FirstOrDefaultAsync(u => u.DeviceId == deviceId);
        }

        public async Task<User> AddAsync(User user)
        {
            await _context.User.AddAsync(user);
            return user;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
