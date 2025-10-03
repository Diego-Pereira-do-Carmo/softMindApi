using SoftMindApi.Entities;

namespace SoftMindApi.Repositories.Interface
{
    public interface IResponseQuestionnaireRepository
    {
        Task AddRangeAsync(IEnumerable<ResponseQuestionnaire> responses);
        Task<bool> SaveChangesAsync();
    }
}
