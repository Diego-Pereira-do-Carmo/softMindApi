using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoftMindApi.DTO;
using SoftMindApi.Services.Interface;

namespace SoftMindApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryQuestionnaireController : ControllerBase
    {
        private readonly ICategoryQuestionnaireService _service;

        public CategoryQuestionnaireController(ICategoryQuestionnaireService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("GetCategoryQuestionnaire")]
        public async Task<IActionResult> GetCategoryQuestionnaire()
        {
            try
            {
                var listCategoryQuestionnaire = await _service.GetCategoriesAsync();
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

            try
            {
                var saved = await _service.AddResponsesAsync(anonymousUserId, model);
                return Ok(saved);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao processar a inserção: {ex.Message}");
            }
        }
    }
}
