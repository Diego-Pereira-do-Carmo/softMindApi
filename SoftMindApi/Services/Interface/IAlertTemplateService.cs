using SoftMindApi.DTO;

namespace SoftMindApi.Services.Interface
{
    public interface IAlertTemplateService
    {
        Task<AlertTemplateDTO> CreateAsync(CreateAlertTemplateDTO dto);
        Task<List<AlertTemplateDTO>> ListAsync();
        Task<bool> DeleteAsync(string id);
    }
}
