using Microsoft.IdentityModel.Tokens;
using SoftMindApi.Configuration;
using SoftMindApi.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SoftMindApi.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<TokenService> _logger;

    public TokenService(JwtSettings jwtSettings, ILogger<TokenService> logger)
    {
        _jwtSettings = jwtSettings;
        _logger = logger;
    }

    public string GenerateToken(string? androidId = null)
    {
        try
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "softmind_app"),
                new Claim("user_type", "mobile_app"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            // Adicionar Android ID se fornecido
            if (!string.IsNullOrEmpty(androidId))
            {
                claims.Add(new Claim("android_id", androidId));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, androidId));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key)
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwtSettings.ExpiryDays),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation(
                "Token gerado{AndroidIdInfo}",
                !string.IsNullOrEmpty(androidId) ? $" para Android ID: {androidId}" : ""
            );

            return tokenString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar token");
            throw;
        }
    }

    public string? GetAndroidIdFromToken(ClaimsPrincipal user)
    {
        return user.FindFirst("android_id")?.Value;
    }
}