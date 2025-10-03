using SoftMindApi.Entities;

namespace SoftMindApi.Repositories.Interface
{
    public interface IAlertTemplateRepository
    {
        Task<List<AlertTemplate>> GetAllAsync();
    }
}
