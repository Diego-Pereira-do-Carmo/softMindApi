using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SoftMindApi.DTO;
using SoftMindApi.Repositories.Interface;
using SoftMindApi.Services;
using SoftMindApi.Services.Interface;
using SoftMindApi.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Xunit;

namespace SoftMindApi.Tests.Services;

public class AuthServiceTests
{
    private static IAuthService CreateService(bool validCreds)
    {
        var userRepo = new Mock<IUserRepository>();
        userRepo.Setup(r => r.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(validCreds);

        var tokenSvc = new Mock<ITokenService>();
        tokenSvc.Setup(s => s.GenerateToken(It.IsAny<string?>())).Returns("token");
        tokenSvc.Setup(s => s.GetAndroidIdFromToken(It.IsAny<ClaimsPrincipal>())).Returns("android-1");

        var logger = new Mock<ILogger<AuthService>>().Object;

        return new AuthService(userRepo.Object, tokenSvc.Object, (ILogger<AuthService>)logger);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredsValid()
    {
        var svc = CreateService(true);
        var res = await svc.LoginAsync(new LoginRequestDTO { Username = "u", Password = "p", AndroidId = "a" });
        res.Should().NotBeNull();
        res!.Token.Should().Be("token");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenCredsInvalid()
    {
        var svc = CreateService(false);
        var res = await svc.LoginAsync(new LoginRequestDTO { Username = "u", Password = "bad" });
        res.Should().BeNull();
    }

    [Fact]
    public void RefreshToken_ShouldReturnNewToken()
    {
        var svc = CreateService(true);
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        var res = svc.RefreshToken(principal);
        res.Token.Should().Be("token");
    }

    [Fact]
    public void VerifyTokenPayload_ShouldReturnExpectedObject()
    {
        var svc = CreateService(true);
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "softmind_app"),
            new Claim("user_type", "mobile_app"),
            new Claim("android_id", "a1")
        });
        var res = svc.VerifyTokenPayload(new ClaimsPrincipal(identity));
        res.Should().NotBeNull();
    }
}
