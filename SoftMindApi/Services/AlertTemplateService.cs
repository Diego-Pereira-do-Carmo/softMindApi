using MongoDB.Bson;
using SoftMindApi.DTO;
using SoftMindApi.Entities;
using SoftMindApi.Repositories.Interface;
using SoftMindApi.Services.Interface;

namespace SoftMindApi.Services
{
    public class AlertTemplateService : IAlertTemplateService
    {
        private readonly IAlertTemplateRepository _repository;

        public AlertTemplateService(IAlertTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<AlertTemplateDTO> CreateAsync(CreateAlertTemplateDTO dto)
        {
            var template = new AlertTemplate
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Message = dto.Message,
                Category = dto.Category
            };

            await _repository.AddAsync(template);
            await _repository.SaveChangesAsync();

            return new AlertTemplateDTO
            {
                Id = template.Id!,
                Message = template.Message,
                Category = template.Category
            };
        }

        public async Task<List<AlertTemplateDTO>> ListAsync()
        {
            var templates = await _repository.GetAllAsync();
            return templates.Select(t => new AlertTemplateDTO
            {
                Id = t.Id!,
                Message = t.Message,
                Category = t.Category
            }).ToList();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var template = await _repository.GetByIdAsync(id);
            if (template == null) return false;

            _repository.Remove(template);
            return await _repository.SaveChangesAsync();
        }
    }
}
