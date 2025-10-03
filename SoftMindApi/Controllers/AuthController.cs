using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoftMindApi.Configuration;
using SoftMindApi.DTO;
using SoftMindApi.Services.Interfaces;
using System.Security.Claims;

namespace SoftMindApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly ApiCredentials _credentials;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        ITokenService tokenService,
        ApiCredentials credentials,
        ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _credentials = credentials;
        _logger = logger;
    }

    /// <summary>
    /// Login único para todos os apps mobile
    /// </summary>
    /// <param name="request">Credenciais de login</param>
    /// <returns>Token JWT</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Login([FromBody] LoginRequestDTO request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar credenciais fixas
            if (request.Username != _credentials.Username ||
                request.Password != _credentials.Password)
            {
                _logger.LogWarning(
                    "Tentativa de login com credenciais inválidas: {Username}",
                    request.Username
                );
                return Unauthorized(new { message = "Credenciais inválidas" });
            }

            _logger.LogInformation(
                "Login bem-sucedido{AndroidIdInfo}",
                !string.IsNullOrEmpty(request.AndroidId)
                    ? $" para Android ID: {request.AndroidId}"
                    : ""
            );

            // Gerar token (opcionalmente com Android ID para tracking)
            var token = _tokenService.GenerateToken(request.AndroidId);
            var expiresAt = DateTime.UtcNow.AddDays(365);

            var response = new TokenResponseDTO
            {
                Token = token,
                AndroidId = request.AndroidId,
                ExpiresAt = expiresAt,
                Message = "Login realizado com sucesso"
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar login");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Renova o token JWT atual
    /// </summary>
    /// <returns>Novo token JWT</returns>
    [HttpPost("refresh")]
    [Authorize]
    [ProducesResponseType(typeof(TokenResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult RefreshToken()
    {
        try
        {
            var androidId = _tokenService.GetAndroidIdFromToken(User);

            var newToken = _tokenService.GenerateToken(androidId);
            var expiresAt = DateTime.UtcNow.AddDays(365);

            var response = new TokenResponseDTO
            {
                Token = newToken,
                AndroidId = androidId,
                ExpiresAt = expiresAt,
                Message = "Token renovado com sucesso"
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao renovar token");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Verifica se o token atual é válido
    /// </summary>
    /// <returns>Informações do token</returns>
    [HttpGet("verify")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult VerifyToken()
    {
        var androidId = _tokenService.GetAndroidIdFromToken(User);
        var userType = User.FindFirst("user_type")?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;

        return Ok(new
        {
            valid = true,
            username = username,
            androidId = androidId,
            userType = userType
        });
    }
}