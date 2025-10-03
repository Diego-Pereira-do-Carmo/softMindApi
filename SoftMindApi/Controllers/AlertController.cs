using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using SoftMindApi.Data;
using SoftMindApi.DTO;
using SoftMindApi.Entities;

namespace SoftMindApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertController : ControllerBase
    {
        private readonly MongoDbContext _context;

        public AlertController(MongoDbContext context)
        {
            _context = context;
        }

        // GET: Gera e retorna um alerta randomizado da biblioteca
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
                // Busca os ids dos alertas ainda não lidos pelo usuário
                var unreadMessages = await _context.Alert
                    .Where(a => a.DeviceId == anonymousUserId && !a.IsRead)
                    .Select(a => a.Message)
                    .ToListAsync();

                // Busca todos os templates disponíveis
                var templates = await _context.AlertTemplates.ToListAsync();

                // Filtra os que o usuário ainda não tem não lido
                var availableTemplates = templates
                    .Where(t => !unreadMessages.Contains(t.Message))
                    .ToList();

                if (availableTemplates.Count == 0)
                {
                    return Ok(new { Message = "Nenhum novo alerta disponível" });
                }

                // Sorteia randomicamente
                var random = new Random();
                var selectedTemplate = availableTemplates[random.Next(availableTemplates.Count)];

                // Cria o alerta real para o usuário
                var newAlert = new Alert
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    DeviceId = anonymousUserId,
                    Message = selectedTemplate.Message,
                    Category = selectedTemplate.Category,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                await _context.Alert.AddAsync(newAlert);
                await _context.SaveChangesAsync();

                // Retorna para o app
                return Ok(new AlertDTO
                {
                    Id = newAlert.Id,
                    Message = newAlert.Message,
                    Category = newAlert.Category,
                    CreatedAt = newAlert.CreatedAt,
                    IsRead = newAlert.IsRead
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao processar: {ex.Message}");
            }
        }


        // GET: Buscar alertas recentes do usuário
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
                var alerts = await _context.Alert
                    .Where(a => a.DeviceId == anonymousUserId)
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(50)
                    .ToListAsync();

                if (alerts == null || alerts.Count == 0)
                {
                    return Ok(new { Message = "Nenhum alerta encontrado" });
                }

                var alertDTOs = alerts.Select(a => new AlertDTO
                {
                    Id = a.Id,
                    Message = a.Message,
                    Category = a.Category,
                    CreatedAt = a.CreatedAt,
                    IsRead = a.IsRead
                }).ToList();

                return Ok(alertDTOs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro interno: {ex.Message}");
            }
        }

        // POST: Marcar alerta como lido
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
                var alert = await _context.Alert
                    .FirstOrDefaultAsync(a => a.Id == dto.AlertId && a.DeviceId == anonymousUserId);

                if (alert == null)
                {
                    return NotFound("Alerta não encontrado");
                }

                alert.IsRead = true;
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Alerta marcado como lido", AlertId = alert.Id });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao processar a atualização: {ex.Message}");
            }
        }

        // POST: Criar novo alerta (útil para testes ou sistema automatizado)
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
                var newAlert = new Alert
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    DeviceId = anonymousUserId,
                    Message = alertDto.Message,
                    Category = alertDto.Category,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                await _context.Alert.AddAsync(newAlert);
                await _context.SaveChangesAsync();

                return Ok(new AlertDTO
                {
                    Id = newAlert.Id,
                    Message = newAlert.Message,
                    Category = newAlert.Category,
                    CreatedAt = newAlert.CreatedAt,
                    IsRead = newAlert.IsRead
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao processar a inserção: {ex.Message}");
            }
        }
    }
}