using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftMindApi.Data;
using SoftMindApi.DTO;
using SoftMindApi.Entities;
using System.Xml.Linq;

namespace SoftMindApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class MoodController : ControllerBase
    {
        private readonly MongoDbContext _context;

        public MoodController(MongoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetMoodLastSevenDays")]
        public async Task<IActionResult> GetMood([FromHeader(Name = "x-device-id")] string anonymousUserId)
        {
            try
            {
                var dataInicial = DateTime.UtcNow.AddDays(-7);
                var moodList = await _context.Mood.Where(m => m.DeviceId == anonymousUserId && m.Data >= dataInicial).OrderBy(m => m.Data).ToListAsync();

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
                var user = await _context.User.FirstOrDefaultAsync(u => u.DeviceId == anonymousUserId);

                if (user == null)
                {
                    User newUser = new User
                    {
                        DeviceId = anonymousUserId,
                    };

                    await _context.User.AddAsync(newUser);
                    await _context.SaveChangesAsync();

                    user = newUser;
                }

                var newMood = new Mood
                {
                    Name = emojiName,
                    DeviceId = user.DeviceId,
                    Data = DateTime.Now,
                };

                await _context.Mood.AddRangeAsync(newMood);
                await _context.SaveChangesAsync();

                return Ok(newMood);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao processar a inserção: {ex.Message}");
            }
        }
    }
}
