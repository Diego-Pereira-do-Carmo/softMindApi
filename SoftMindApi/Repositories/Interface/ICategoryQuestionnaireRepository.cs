using SoftMindApi.Entities;

namespace SoftMindApi.Repositories.Interface
{
    public interface ICategoryQuestionnaireRepository
    {
        Task<List<CategoryQuestionnaire>> GetCategoryQuestionnaires();
    }
}
