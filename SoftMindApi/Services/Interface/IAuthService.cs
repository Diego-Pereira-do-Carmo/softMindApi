using System.Security.Claims;
using SoftMindApi.DTO;

namespace SoftMindApi.Services.Interface
{
    public interface IAuthService
    {
        Task<TokenResponseDTO?> LoginAsync(LoginRequestDTO request);
        TokenResponseDTO RefreshToken(ClaimsPrincipal user);
        object VerifyTokenPayload(ClaimsPrincipal user);
    }
}
