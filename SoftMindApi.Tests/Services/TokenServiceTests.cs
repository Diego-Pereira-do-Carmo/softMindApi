using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SoftMindApi.Configuration;
using SoftMindApi.Services;
using System.Security.Claims;
using Xunit;

namespace SoftMindApi.Tests.Services;

public class TokenServiceTests
{
    private static TokenService CreateService()
    {
        var settings = new JwtSettings
        {
            Issuer = "issuer",
            Audience = "aud",
            Key = new string('k', 64),
            ExpiryDays = 1
        };
        var logger = new Mock<ILogger<TokenService>>().Object;
        return new TokenService(settings, (ILogger<TokenService>)logger);
    }

    [Fact]
    public void GenerateToken_ShouldReturnToken_WithAndroidIdClaim()
    {
        var svc = CreateService();
        var token = svc.GenerateToken("android-123");
        token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GetAndroidIdFromToken_WhenClaimMissing_ReturnsNull()
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "x") });
        var user = new ClaimsPrincipal(identity);
        var svc = CreateService();
        svc.GetAndroidIdFromToken(user).Should().BeNull();
    }
}
