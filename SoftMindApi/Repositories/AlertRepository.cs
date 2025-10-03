using Microsoft.EntityFrameworkCore;
using SoftMindApi.Data;
using SoftMindApi.Entities;
using SoftMindApi.Repositories.Interface;

namespace SoftMindApi.Repositories
{
    public class AlertRepository : IAlertRepository
    {
        private readonly MongoDbContext _context;

        public AlertRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetUnreadMessagesByDeviceAsync(string deviceId)
        {
            return await _context.Alert
                .Where(a => a.DeviceId == deviceId && !a.IsRead)
                .Select(a => a.Message)
                .ToListAsync();
        }

        public async Task<List<Alert>> GetRecentAlertsByDeviceAsync(string deviceId, int take = 50)
        {
            return await _context.Alert
                .Where(a => a.DeviceId == deviceId)
                .OrderByDescending(a => a.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<Alert?> GetByIdForDeviceAsync(string alertId, string deviceId)
        {
            return await _context.Alert
                .FirstOrDefaultAsync(a => a.Id == alertId && a.DeviceId == deviceId);
        }

        public async Task<Alert> AddAsync(Alert alert)
        {
            await _context.Alert.AddAsync(alert);
            return alert;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
