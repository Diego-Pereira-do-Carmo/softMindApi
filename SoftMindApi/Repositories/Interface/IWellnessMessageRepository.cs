using SoftMindApi.Entities;

namespace SoftMindApi.Repositories.Interface
{
    public interface IWellnessMessageRepository
    {
        Task<List<WellnessMessage>> GetAllActiveAsync();
        Task<bool> SaveChangesAsync();
    }
}
