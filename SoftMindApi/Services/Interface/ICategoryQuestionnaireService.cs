using SoftMindApi.DTO;
using SoftMindApi.Entities;

namespace SoftMindApi.Services.Interface
{
    public interface ICategoryQuestionnaireService
    {
        Task<List<CategoryQuestionnaire>> GetCategoriesAsync();
        Task<List<ResponseQuestionnaire>> AddResponsesAsync(string deviceId, List<ResponseQuestionnaireDTO> model);
    }
}
