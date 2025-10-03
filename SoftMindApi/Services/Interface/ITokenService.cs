using System.Security.Claims;

namespace SoftMindApi.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(string? androidId = null);
    string? GetAndroidIdFromToken(ClaimsPrincipal user);
}