using Microsoft.EntityFrameworkCore;
using SoftMindApi.Data;
using SoftMindApi.Entities;
using SoftMindApi.Repositories.Interface;

namespace SoftMindApi.Repositories
{
    public class WellnessMessageRepository : IWellnessMessageRepository
    {
        private readonly MongoDbContext _context;

        public WellnessMessageRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<WellnessMessage>> GetAllActiveAsync()
        {
            return await _context.WellnessMessages
                .Where(w => w.Active)
                .ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
