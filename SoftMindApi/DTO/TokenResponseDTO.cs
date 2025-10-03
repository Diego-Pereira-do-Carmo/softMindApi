namespace SoftMindApi.DTO;

public class TokenResponseDTO
{
    public string Token { get; set; } = string.Empty;
    public string? AndroidId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Message { get; set; } = string.Empty;
}