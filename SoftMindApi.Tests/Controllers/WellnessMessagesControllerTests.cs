using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SoftMindApi.Controllers;
using SoftMindApi.DTO;
using SoftMindApi.Services.Interface;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;

namespace SoftMindApi.Tests.Controllers;

public class WellnessMessagesControllerTests
{
    [Fact]
    public async Task GetRandom_ShouldReturnBadRequest_WhenNoHeader()
    {
        var ctrl = new WellnessMessagesController(Mock.Of<IWellnessMessageService>());
        var res = await ctrl.GetRandom("");
        res.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetRandom_ShouldReturnNoContent_WhenEmpty()
    {
        var svc = new Mock<IWellnessMessageService>();
        svc.Setup(s => s.GetRandomForDeviceAsync("d1", It.IsAny<int>())).ReturnsAsync(new List<WellnessMessageDTO>());
        var ctrl = new WellnessMessagesController(svc.Object);
        var res = await ctrl.GetRandom("d1");
        res.Should().BeOfType<NoContentResult>();
    }
}
