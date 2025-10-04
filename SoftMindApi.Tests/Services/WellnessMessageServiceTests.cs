using FluentAssertions;
using Moq;
using SoftMindApi.Entities;
using SoftMindApi.Repositories.Interface;
using SoftMindApi.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SoftMindApi.Tests.Services;

public class WellnessMessageServiceTests
{
    [Fact]
    public async Task GetRandomForDevice_ShouldUpdateReadStats_AndReturnDtos()
    {
        var messages = new List<WellnessMessage>
        {
            new WellnessMessage { Id = MongoDB.Bson.ObjectId.GenerateNewId(), Name = "A", Active = true },
            new WellnessMessage { Id = MongoDB.Bson.ObjectId.GenerateNewId(), Name = "B", Active = true }
        };
        var repo = new Mock<IWellnessMessageRepository>();
        repo.Setup(r => r.GetAllActiveAsync()).ReturnsAsync(messages);
        repo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var svc = new WellnessMessageService(repo.Object);
        var res = await svc.GetRandomForDeviceAsync("dev1", 2);
        res.Should().NotBeEmpty();
        messages.Any(m => m.ReadStats.Any(s => s.DeviceId == "dev1")).Should().BeTrue();
    }
}
