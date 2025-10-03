using MongoDB.Bson;
using SoftMindApi.DTO;
using SoftMindApi.Entities;
using SoftMindApi.Repositories.Interface;
using SoftMindApi.Services.Interface;

namespace SoftMindApi.Services
{
    public class AlertService : IAlertService
    {
        private readonly IAlertRepository _alertRepository;
        private readonly IAlertTemplateRepository _templateRepository;

        public AlertService(
            IAlertRepository alertRepository,
            IAlertTemplateRepository templateRepository)
        {
            _alertRepository = alertRepository;
            _templateRepository = templateRepository;
        }

        public async Task<AlertDTO?> GetRandomAlertAsync(string deviceId)
        {
            var unreadMessages = await _alertRepository.GetUnreadMessagesByDeviceAsync(deviceId);
            var templates = await _templateRepository.GetAllAsync();

            var availableTemplates = templates
                .Where(t => !unreadMessages.Contains(t.Message))
                .ToList();

            if (availableTemplates.Count == 0)
            {
                return null;
            }

            var random = new Random();
            var selectedTemplate = availableTemplates[random.Next(availableTemplates.Count)];

            var newAlert = new Alert
            {
                Id = ObjectId.GenerateNewId().ToString(),
                DeviceId = deviceId,
                Message = selectedTemplate.Message,
                Category = selectedTemplate.Category,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _alertRepository.AddAsync(newAlert);
            await _alertRepository.SaveChangesAsync();

            return new AlertDTO
            {
                Id = newAlert.Id!,
                Message = newAlert.Message,
                Category = newAlert.Category,
                CreatedAt = newAlert.CreatedAt,
                IsRead = newAlert.IsRead
            };
        }

        public async Task<List<AlertDTO>> GetRecentAlertsAsync(string deviceId, int take = 50)
        {
            var alerts = await _alertRepository.GetRecentAlertsByDeviceAsync(deviceId, take);
            return alerts.Select(a => new AlertDTO
            {
                Id = a.Id!,
                Message = a.Message,
                Category = a.Category,
                CreatedAt = a.CreatedAt,
                IsRead = a.IsRead
            }).ToList();
        }

        public async Task<bool> MarkAsReadAsync(string deviceId, string alertId)
        {
            var alert = await _alertRepository.GetByIdForDeviceAsync(alertId, deviceId);
            if (alert == null)
                return false;

            alert.IsRead = true;
            return await _alertRepository.SaveChangesAsync();
        }

        public async Task<AlertDTO> CreateAlertAsync(string deviceId, CreateAlertDTO dto)
        {
            var newAlert = new Alert
            {
                Id = ObjectId.GenerateNewId().ToString(),
                DeviceId = deviceId,
                Message = dto.Message,
                Category = dto.Category,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _alertRepository.AddAsync(newAlert);
            await _alertRepository.SaveChangesAsync();

            return new AlertDTO
            {
                Id = newAlert.Id!,
                Message = newAlert.Message,
                Category = newAlert.Category,
                CreatedAt = newAlert.CreatedAt,
                IsRead = newAlert.IsRead
            };
        }
    }
}
