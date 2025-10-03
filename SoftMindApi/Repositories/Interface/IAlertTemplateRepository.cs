using SoftMindApi.Entities;

namespace SoftMindApi.Repositories.Interface
{
    public interface IAlertTemplateRepository
    {
        Task<List<AlertTemplate>> GetAllAsync();
        Task<AlertTemplate> AddAsync(AlertTemplate template);
        Task<AlertTemplate?> GetByIdAsync(string id);
        void Remove(AlertTemplate template);
        Task<bool> SaveChangesAsync();
    }
}
