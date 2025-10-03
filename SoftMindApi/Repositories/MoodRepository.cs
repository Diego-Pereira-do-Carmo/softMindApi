using Microsoft.EntityFrameworkCore;
using SoftMindApi.Data;
using SoftMindApi.Entities;
using SoftMindApi.Repositories.Interface;

namespace SoftMindApi.Repositories
{
    public class MoodRepository : IMoodRepository
    {
        private readonly MongoDbContext _context;

        public MoodRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Mood>> GetLastSevenDaysAsync(string deviceId)
        {
            var start = DateTime.UtcNow.AddDays(-7);
            return await _context.Mood
                .Where(m => m.DeviceId == deviceId && m.Data >= start)
                .OrderBy(m => m.Data)
                .ToListAsync();
        }

        public async Task AddAsync(Mood mood)
        {
            await _context.Mood.AddAsync(mood);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
