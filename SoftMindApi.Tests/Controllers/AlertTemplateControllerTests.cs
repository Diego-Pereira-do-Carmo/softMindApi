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

public class AlertTemplateControllerTests
{
    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenMessageEmpty()
    {
        var ctrl = new AlertTemplateController(Mock.Of<IAlertTemplateService>());
        var res = await ctrl.Create(new CreateAlertTemplateDTO { Message = "", Category = "c" });
        res.Should().BeOfType<BadRequestObjectResult>();
    }
}
