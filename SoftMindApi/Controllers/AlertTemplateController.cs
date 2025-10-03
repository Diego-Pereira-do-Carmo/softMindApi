using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoftMindApi.DTO;
using SoftMindApi.Services.Interface;

namespace SoftMindApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlertTemplateController : ControllerBase
    {
        private readonly IAlertTemplateService _service;

        public AlertTemplateController(IAlertTemplateService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CreateAlertTemplateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest("Mensagem inválida");

            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpGet]
        [Route("List")]
        public async Task<IActionResult> List()
        {
            var result = await _service.ListAsync();
            return Ok(result);
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var removed = await _service.DeleteAsync(id);
            if (!removed)
                return NotFound("Template não encontrado");

            return Ok(new { Message = "Template removido com sucesso" });
        }
    }
}
