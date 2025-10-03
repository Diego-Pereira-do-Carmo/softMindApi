using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoftMindApi.DTO;
using SoftMindApi.Services.Interface;

namespace SoftMindApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;

        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        [HttpGet]
        [Route("GetRandomAlert")]
        public async Task<IActionResult> GetRandomAlert([FromHeader(Name = "x-device-id")] string anonymousUserId)
        {
            if (string.IsNullOrWhiteSpace(anonymousUserId))
            {
                return BadRequest("DeviceId inválido");
            }

            try
            {
                var alert = await _alertService.GetRandomAlertAsync(anonymousUserId);
                if (alert == null)
                {
                    return Ok(new { Message = "Nenhum novo alerta disponível" });
                }
                return Ok(alert);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao processar: {ex.Message}");
            }
        }


        [HttpGet]
        [Route("GetRecentAlerts")]
        public async Task<IActionResult> GetRecentAlerts([FromHeader(Name = "x-device-id")] string anonymousUserId)
        {
            if (string.IsNullOrWhiteSpace(anonymousUserId))
            {
                return BadRequest("DeviceId inválido");
            }

            try
            {
                var alerts = await _alertService.GetRecentAlertsAsync(anonymousUserId);
                if (alerts == null || alerts.Count == 0)
                {
                    return Ok(new { Message = "Nenhum alerta encontrado" });
                }
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("MarkAsRead")]
        public async Task<IActionResult> MarkAsRead(
            [FromHeader(Name = "x-device-id")] string anonymousUserId,
            [FromBody] MarkAsReadDTO dto)
        {
            if (string.IsNullOrWhiteSpace(anonymousUserId) || string.IsNullOrWhiteSpace(dto.AlertId))
            {
                return BadRequest("Dados inválidos");
            }

            try
            {
                var updated = await _alertService.MarkAsReadAsync(anonymousUserId, dto.AlertId);
                if (!updated)
                {
                    return NotFound("Alerta não encontrado");
                }
                return Ok(new { Message = "Alerta marcado como lido", AlertId = dto.AlertId });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao processar a atualização: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("CreateAlert")]
        public async Task<IActionResult> CreateAlert(
            [FromHeader(Name = "x-device-id")] string anonymousUserId,
            [FromBody] CreateAlertDTO alertDto)
        {
            if (string.IsNullOrWhiteSpace(anonymousUserId) || string.IsNullOrWhiteSpace(alertDto.Message))
            {
                return BadRequest("Dados inválidos");
            }

            try
            {
                var created = await _alertService.CreateAlertAsync(anonymousUserId, alertDto);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao processar a inserção: {ex.Message}");
            }
        }
    }
}