using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class AlertTemplateController : ControllerBase
    {
        private readonly MongoDbContext _context;

        public AlertTemplateController(MongoDbContext context)
        {
            _context = context;
        }

        // POST: Criar novo template de alerta
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CreateAlertTemplateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest("Mensagem inválida");

            var template = new AlertTemplate
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Message = dto.Message,
                Category = dto.Category
            };

            await _context.AlertTemplates.AddAsync(template);
            await _context.SaveChangesAsync();

            return Ok(new AlertTemplateDTO
            {
                Id = template.Id!,
                Message = template.Message,
                Category = template.Category
            });
        }

        // GET: Listar todos os templates
        [HttpGet]
        [Route("List")]
        public async Task<IActionResult> List()
        {
            var templates = await _context.AlertTemplates.ToListAsync();

            var result = templates.Select(t => new AlertTemplateDTO
            {
                Id = t.Id!,
                Message = t.Message,
                Category = t.Category
            });

            return Ok(result);
        }

        // DELETE: Remover template
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var template = await _context.AlertTemplates.FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
                return NotFound("Template não encontrado");

            _context.AlertTemplates.Remove(template);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Template removido com sucesso" });
        }
    }
}
