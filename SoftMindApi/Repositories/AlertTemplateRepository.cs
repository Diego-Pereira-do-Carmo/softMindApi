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

        public async Task<AlertTemplate> AddAsync(AlertTemplate template)
        {
            await _context.AlertTemplates.AddAsync(template);
            return template;
        }

        public async Task<AlertTemplate?> GetByIdAsync(string id)
        {
            return await _context.AlertTemplates.FirstOrDefaultAsync(t => t.Id == id);
        }

        public void Remove(AlertTemplate template)
        {
            _context.AlertTemplates.Remove(template);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
