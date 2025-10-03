using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftMindApi.Data;
using SoftMindApi.DTO;
using SoftMindApi.Entities;

namespace SoftMindApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryQuestionnaireController : ControllerBase
    {
        private readonly MongoDbContext _context;

        public CategoryQuestionnaireController(MongoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetCategoryQuestionnaire")]
        public async Task<IActionResult> GetCategoryQuestionnaire()
        {
            try
            {
                List<CategoryQuestionnaire> listCategoryQuestionnaire = await _context.CategoryQuestionnaire.ToListAsync();

                if (listCategoryQuestionnaire == null || listCategoryQuestionnaire.Count == 0)
                {
                    return Ok(new { Message = "Nenhum questionario encontrado" });
                }

                return Ok(listCategoryQuestionnaire);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("AddResponseQuestionnaire")]
        public async Task<IActionResult> PostResponseQuestionnaire([FromHeader(Name = "x-device-id")] string anonymousUserId, [FromBody] List<ResponseQuestionnaireDTO> model)
        {
            if (string.IsNullOrWhiteSpace(anonymousUserId) || model == null || model.Count == 0)
            {
                return BadRequest("Dados inválido");
            }

            var user = await _context.User.FirstOrDefaultAsync(u => u.DeviceId == anonymousUserId);

            if (user == null)
            {
                User newUser = new User
                {
                    DeviceId = anonymousUserId,
                };

                await _context.User.AddAsync(newUser);
                await _context.SaveChangesAsync();
            }

            try
            {
                List<ResponseQuestionnaire> responsesToSave = new List<ResponseQuestionnaire>();
                foreach (var response in model)
                {
                    var newResponse = new ResponseQuestionnaire
                    {
                        pergunta = response.pergunta,
                        resposta = response.resposta,
                        Data = DateTime.Now,
                        DeviceId = anonymousUserId
                    };

                    responsesToSave.Add(newResponse);
                }

                await _context.ResponseQuestionnaire.AddRangeAsync(responsesToSave);
                await _context.SaveChangesAsync();

                return Ok(responsesToSave);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao processar a inserção: {ex.Message}");
            }
        }
    }
}
