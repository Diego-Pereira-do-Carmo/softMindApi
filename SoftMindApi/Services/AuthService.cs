using SoftMindApi.DTO;
using SoftMindApi.Repositories.Interface;
using SoftMindApi.Services.Interfaces;
using SoftMindApi.Services.Interface;
using System.Security.Claims;

namespace SoftMindApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<TokenResponseDTO?> LoginAsync(LoginRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return null;
            }

            var isValid = await _userRepository.ValidateCredentialsAsync(request.Username, request.Password);
            if (!isValid)
            {
                _logger.LogWarning("Tentativa de login inválida para {Username}", request.Username);
                return null;
            }

            var token = _tokenService.GenerateToken(request.AndroidId);
            var expiresAt = DateTime.UtcNow.AddDays(365);

            return new TokenResponseDTO
            {
                Token = token,
                AndroidId = request.AndroidId,
                ExpiresAt = expiresAt,
                Message = "Login realizado com sucesso"
            };
        }

        public TokenResponseDTO RefreshToken(ClaimsPrincipal user)
        {
            var androidId = _tokenService.GetAndroidIdFromToken(user);
            var newToken = _tokenService.GenerateToken(androidId);
            var expiresAt = DateTime.UtcNow.AddDays(365);

            return new TokenResponseDTO
            {
                Token = newToken,
                AndroidId = androidId,
                ExpiresAt = expiresAt,
                Message = "Token renovado com sucesso"
            };
        }

        public object VerifyTokenPayload(ClaimsPrincipal user)
        {
            var androidId = _tokenService.GetAndroidIdFromToken(user);
            var userType = user.FindFirst("user_type")?.Value;
            var username = user.FindFirst(ClaimTypes.Name)?.Value;

            return new
            {
                valid = true,
                username,
                androidId,
                userType
            };
        }
    }
}
