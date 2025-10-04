using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SoftMindApi.Controllers;
using SoftMindApi.Entities;
using SoftMindApi.Services.Interface;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;

namespace SoftMindApi.Tests.Controllers;

public class MoodControllerTests
{
    [Fact]
    public async Task AddMood_ShouldReturnBadRequest_WhenInvalidArgs()
    {
        var ctrl = new MoodController(Mock.Of<IMoodService>());
        var res = await ctrl.PostMood("", "");
        res.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetMood_ShouldReturnMessage_WhenEmpty()
    {
        var svc = new Mock<IMoodService>();
        svc.Setup(s => s.GetMoodLastSevenDaysAsync("d1")).ReturnsAsync(new List<Mood>());
        var ctrl = new MoodController(svc.Object);
        var res = await ctrl.GetMood("d1") as OkObjectResult;
        res.Should().NotBeNull();
    }
}
