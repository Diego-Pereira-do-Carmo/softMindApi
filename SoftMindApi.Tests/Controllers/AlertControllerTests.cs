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

public class AlertControllerTests
{
    [Fact]
    public async Task GetRandomAlert_ShouldReturnBadRequest_WhenHeaderMissing()
    {
        var ctrl = new AlertController(Mock.Of<IAlertService>());
        var res = await ctrl.GetRandomAlert("");
        res.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetRecentAlerts_ShouldReturnMessage_WhenEmpty()
    {
        var svc = new Mock<IAlertService>();
        svc.Setup(s => s.GetRecentAlertsAsync("d1", It.IsAny<int>()))
            .ReturnsAsync(new List<AlertDTO>());
        var ctrl = new AlertController(svc.Object);
        var res = await ctrl.GetRecentAlerts("d1") as OkObjectResult;
        res.Should().NotBeNull();
    }
}
