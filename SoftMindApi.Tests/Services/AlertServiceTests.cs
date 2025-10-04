using FluentAssertions;
using Moq;
using SoftMindApi.DTO;
using SoftMindApi.Entities;
using SoftMindApi.Repositories.Interface;
using SoftMindApi.Services;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Xunit;

namespace SoftMindApi.Tests.Services;

public class AlertServiceTests
{
    [Fact]
    public async Task GetRandomAlert_WhenNoTemplatesAvailable_ReturnsNull()
    {
        var alertRepo = new Mock<IAlertRepository>();
        alertRepo.Setup(r => r.GetUnreadMessagesByDeviceAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<string>());
        var tmplRepo = new Mock<IAlertTemplateRepository>();
        tmplRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<AlertTemplate>());

        var svc = new AlertService(alertRepo.Object, tmplRepo.Object);
        var res = await svc.GetRandomAlertAsync("d1");
        res.Should().BeNull();
    }

    [Fact]
    public async Task CreateAlert_ShouldReturnDto()
    {
        var alertRepo = new Mock<IAlertRepository>();
        alertRepo.Setup(r => r.AddAsync(It.IsAny<Alert>())).ReturnsAsync((Alert a) => a);
        alertRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        var tmplRepo = new Mock<IAlertTemplateRepository>();
        var svc = new AlertService(alertRepo.Object, tmplRepo.Object);

        var dto = await svc.CreateAlertAsync("dev1", new CreateAlertDTO { Message = "m", Category = "c" });
        dto.Message.Should().Be("m");
        dto.Category.Should().Be("c");
    }

    [Fact]
    public async Task GetRecentAlerts_ShouldMapDtos()
    {
        var alertRepo = new Mock<IAlertRepository>();
        alertRepo.Setup(r => r.GetRecentAlertsByDeviceAsync("d1", It.IsAny<int>()))
            .ReturnsAsync(new List<Alert> { new Alert { Id = "1", Message = "m", Category = "c", CreatedAt = DateTime.UtcNow } });
        var svc = new AlertService(alertRepo.Object, Mock.Of<IAlertTemplateRepository>());
        var res = await svc.GetRecentAlertsAsync("d1");
        res.Should().HaveCount(1);
        res[0].Message.Should().Be("m");
    }

    [Fact]
    public async Task MarkAsRead_WhenNotFound_ReturnsFalse()
    {
        var alertRepo = new Mock<IAlertRepository>();
        alertRepo.Setup(r => r.GetByIdForDeviceAsync("a1", "d1")).ReturnsAsync((Alert?)null);
        var svc = new AlertService(alertRepo.Object, Mock.Of<IAlertTemplateRepository>());
        var ok = await svc.MarkAsReadAsync("d1", "a1");
        ok.Should().BeFalse();
    }
}
