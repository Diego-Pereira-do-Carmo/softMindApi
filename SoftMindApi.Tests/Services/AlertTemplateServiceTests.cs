using FluentAssertions;
using Moq;
using SoftMindApi.DTO;
using SoftMindApi.Entities;
using SoftMindApi.Repositories.Interface;
using SoftMindApi.Services;
using System.Threading.Tasks;
using Xunit;

namespace SoftMindApi.Tests.Services;

public class AlertTemplateServiceTests
{
    [Fact]
    public async Task Create_ShouldReturnDto()
    {
        var repo = new Mock<IAlertTemplateRepository>();
        repo.Setup(r => r.AddAsync(It.IsAny<AlertTemplate>())).ReturnsAsync((AlertTemplate a) => a);
        repo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        var svc = new AlertTemplateService(repo.Object);

        var res = await svc.CreateAsync(new CreateAlertTemplateDTO { Message = "m1", Category = "c1" });
        res.Message.Should().Be("m1");
    }

    [Fact]
    public async Task Delete_WhenNotFound_ReturnsFalse()
    {
        var repo = new Mock<IAlertTemplateRepository>();
        repo.Setup(r => r.GetByIdAsync("x")).ReturnsAsync((AlertTemplate?)null);
        var svc = new AlertTemplateService(repo.Object);
        (await svc.DeleteAsync("x")).Should().BeFalse();
    }
}
