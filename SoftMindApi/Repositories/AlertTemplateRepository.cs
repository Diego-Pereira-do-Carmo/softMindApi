using Microsoft.EntityFrameworkCore;
using SoftMindApi.Data;
using SoftMindApi.Entities;
using SoftMindApi.Repositories.Interface;

namespace SoftMindApi.Repositories
{
    public class AlertTemplateRepository : IAlertTemplateRepository
    {
        private readonly MongoDbContext _context;

        public AlertTemplateRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<AlertTemplate>> GetAllAsync()
        {
            return await _context.AlertTemplates.ToListAsync();
        }
    }
}
