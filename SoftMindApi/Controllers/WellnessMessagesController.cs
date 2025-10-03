using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoftMindApi.DTO;
using SoftMindApi.Services.Interface;

namespace SoftMindApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WellnessMessagesController : ControllerBase
    {
        private readonly IWellnessMessageService _service;

        public WellnessMessagesController(IWellnessMessageService service)
        {
            _service = service;
        }

        [HttpGet("GetRandom")]
        public async Task<IActionResult> GetRandom(
            [FromHeader(Name = "x-device-id")] string? deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return BadRequest("Missing device id. Provide X-Device-Id header.");
            }

            try
            {
                var dtoList = await _service.GetRandomForDeviceAsync(deviceId);
                if (dtoList.Count == 0)
                {
                    return NoContent();
                }
                return Ok(dtoList);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing request: {ex.Message}");
            }
        }
    }
}
