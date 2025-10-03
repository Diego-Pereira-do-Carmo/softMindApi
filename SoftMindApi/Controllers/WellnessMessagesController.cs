using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftMindApi.Data;
using SoftMindApi.DTO;
using SoftMindApi.Entities;

namespace SoftMindApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WellnessMessagesController : ControllerBase
    {
        private readonly MongoDbContext _context;

        public WellnessMessagesController(MongoDbContext context)
        {
            _context = context;
        }

        // Resolve Brazil timezone in Linux/Windows; fallback to local if not found
        private static TimeZoneInfo GetBrazilTimeZone()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo"); } catch { }
            try { return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"); } catch { }
            return TimeZoneInfo.Local;
        }

        /// <summary>
        /// Returns a random active wellness message and records the read for this device.
        /// Device id is read from the X-Device-Id header (Android ID).
        /// </summary>
        /// <param name="deviceId">Android ID from header X-Device-Id</param>
        [HttpGet("GetRandom")]
        public async Task<IActionResult> GetRandom(
            [FromHeader(Name = "x-device-id")] string? deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return BadRequest("Missing device id. Provide X-Device-Id header.");
            }

            // Normalize to lowercase to avoid case-sensitive duplicates
            var deviceKey = deviceId.Trim().ToLowerInvariant();

            try
            {
                var tz = GetBrazilTimeZone();
                var nowBr = TimeZoneInfo.ConvertTime(DateTime.Now, tz);

                var allActive = await _context.WellnessMessages
                    .Where(w => w.Active)
                    .ToListAsync();

                // No 7-day filter for now: any active message is a candidate
                var candidates = allActive;

                if (candidates.Count == 0)
                {
                    return NoContent();
                }

                var random = new Random();
                var chosen = candidates[random.Next(candidates.Count)];

                var readStat = chosen.ReadStats.FirstOrDefault(s => s.DeviceId == deviceKey);
                if (readStat == null)
                {
                    chosen.ReadStats.Add(new WellnessReadStat
                    {
                        DeviceId = deviceKey,
                        Count = 1,
                        LastReadAt = nowBr
                    });
                }
                else
                {
                    readStat.Count += 1;
                    readStat.LastReadAt = nowBr;
                }

                await _context.SaveChangesAsync();

                var dto = new WellnessMessageDTO
                {
                    Id = chosen.Id.ToString(),
                    Name = chosen.Name
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing request: {ex.Message}");
            }
        }
    }
}
