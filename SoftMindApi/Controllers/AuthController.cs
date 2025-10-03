using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoftMindApi.DTO;
using SoftMindApi.Services.Interface;
using System.Security.Claims;

namespace SoftMindApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.LoginAsync(request);
            if (response == null)
            {
                return Unauthorized(new { message = "Credenciais inválidas" });
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar login");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpPost("refresh")]
    [Authorize]
    [ProducesResponseType(typeof(TokenResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult RefreshToken()
    {
        try
        {
            var response = _authService.RefreshToken(User);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao renovar token");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpGet("verify")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult VerifyToken()
    {
        var payload = _authService.VerifyTokenPayload(User);
        return Ok(payload);
    }
}