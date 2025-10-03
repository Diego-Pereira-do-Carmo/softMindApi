using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoftMindApi.Services.Interface;

namespace SoftMindApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class MoodController : ControllerBase
    {
        private readonly IMoodService _moodService;

        public MoodController(IMoodService moodService)
        {
            _moodService = moodService;
        }

        [HttpGet]
        [Route("GetMoodLastSevenDays")]
        public async Task<IActionResult> GetMood([FromHeader(Name = "x-device-id")] string anonymousUserId)
        {
            try
            {
                var moodList = await _moodService.GetMoodLastSevenDaysAsync(anonymousUserId);
                if (moodList == null || moodList.Count == 0)
                {
                    return Ok(new { Message = "Nenhum registro encontrado" });
                }
                return Ok(moodList);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro interno: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("AddMood")]
        public async Task<IActionResult> PostMood([FromHeader(Name = "x-device-id")] string anonymousUserId, [FromBody] string emojiName)
        {
            if (string.IsNullOrWhiteSpace(anonymousUserId) || string.IsNullOrWhiteSpace(emojiName))
            {
                return BadRequest("Dados inválido");
            }

            try
            {
                var newMood = await _moodService.AddMoodAsync(anonymousUserId, emojiName);
                return Ok(newMood);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao processar a inserção: {ex.Message}");
            }
        }
    }
}
