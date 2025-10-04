using FluentAssertions;
using Moq;
using SoftMindApi.Entities;
using SoftMindApi.Repositories.Interface;
using SoftMindApi.Services;
using System.Threading.Tasks;
using Xunit;

namespace SoftMindApi.Tests.Services;

public class MoodServiceTests
{
    [Fact]
    public async Task AddMood_ShouldCreateUser_WhenMissing()
    {
        var moodRepo = new Mock<IMoodRepository>();
        moodRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        var userRepo = new Mock<IUserRepository>();
        userRepo.Setup(r => r.GetByDeviceIdAsync("d1")).ReturnsAsync((User?)null);
        userRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);
        userRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        var svc = new MoodService(moodRepo.Object, userRepo.Object);

        var mood = await svc.AddMoodAsync("d1", "happy");
        mood.Name.Should().Be("happy");
        mood.DeviceId.Should().Be("d1");
    }
}
