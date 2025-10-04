using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SoftMindApi.Controllers;
using SoftMindApi.DTO;
using SoftMindApi.Services.Interface;
using System.Threading.Tasks;
using Xunit;

namespace SoftMindApi.Tests.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenNull()
    {
        var svc = new Mock<IAuthService>();
        svc.Setup(s => s.LoginAsync(It.IsAny<LoginRequestDTO>())).ReturnsAsync((TokenResponseDTO?)null);
        var ctrl = new AuthController(svc.Object, Mock.Of<Microsoft.Extensions.Logging.ILogger<AuthController>>());
        var res = await ctrl.Login(new LoginRequestDTO { Username = "u", Password = "p" });
        (res as UnauthorizedObjectResult).Should().NotBeNull();
    }
}
