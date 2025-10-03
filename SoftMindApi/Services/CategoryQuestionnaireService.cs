using SoftMindApi.DTO;
using SoftMindApi.Entities;
using SoftMindApi.Repositories.Interface;
using SoftMindApi.Services.Interface;

namespace SoftMindApi.Services
{
    public class CategoryQuestionnaireService : ICategoryQuestionnaireService
    {
        private readonly ICategoryQuestionnaireRepository _categoryRepo;
        private readonly IResponseQuestionnaireRepository _responseRepo;
        private readonly IUserRepository _userRepo;

        public CategoryQuestionnaireService(
            ICategoryQuestionnaireRepository categoryRepo,
            IResponseQuestionnaireRepository responseRepo,
            IUserRepository userRepo)
        {
            _categoryRepo = categoryRepo;
            _responseRepo = responseRepo;
            _userRepo = userRepo;
        }

        public async Task<List<CategoryQuestionnaire>> GetCategoriesAsync()
        {
            return await _categoryRepo.GetCategoryQuestionnaires();
        }

        public async Task<List<ResponseQuestionnaire>> AddResponsesAsync(string deviceId, List<ResponseQuestionnaireDTO> model)
        {
            var user = await _userRepo.GetByDeviceIdAsync(deviceId);
            if (user == null)
            {
                user = new User { DeviceId = deviceId };
                await _userRepo.AddAsync(user);
                await _userRepo.SaveChangesAsync();
            }

            var responsesToSave = model.Select(m => new ResponseQuestionnaire
            {
                DeviceId = deviceId,
                pergunta = m.pergunta,
                resposta = m.resposta,
                Data = DateTime.Now
            }).ToList();

            await _responseRepo.AddRangeAsync(responsesToSave);
            await _responseRepo.SaveChangesAsync();

            return responsesToSave;
        }
    }
}
