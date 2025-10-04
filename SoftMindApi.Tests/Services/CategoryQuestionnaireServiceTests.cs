using FluentAssertions;
using Moq;
using SoftMindApi.DTO;
using SoftMindApi.Entities;
using SoftMindApi.Repositories.Interface;
using SoftMindApi.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;

namespace SoftMindApi.Tests.Services;

public class CategoryQuestionnaireServiceTests
{
    [Fact]
    public async Task GetCategories_ShouldReturnFromRepository()
    {
        var categoryRepo = new Mock<ICategoryQuestionnaireRepository>();
        categoryRepo.Setup(r => r.GetCategoryQuestionnaires()).ReturnsAsync(new List<CategoryQuestionnaire> { new CategoryQuestionnaire() });
        var svc = new CategoryQuestionnaireService(categoryRepo.Object, Mock.Of<IResponseQuestionnaireRepository>(), Mock.Of<IUserRepository>());
        var res = await svc.GetCategoriesAsync();
        res.Should().HaveCount(1);
    }

    [Fact]
    public async Task AddResponses_ShouldEnsureUserAndPersist()
    {
        var categoryRepo = new Mock<ICategoryQuestionnaireRepository>();
        var respRepo = new Mock<IResponseQuestionnaireRepository>();
        respRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        var userRepo = new Mock<IUserRepository>();
        userRepo.Setup(r => r.GetByDeviceIdAsync("d1")).ReturnsAsync((User?)null);
        userRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);
        userRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var svc = new CategoryQuestionnaireService(categoryRepo.Object, respRepo.Object, userRepo.Object);
        var list = await svc.AddResponsesAsync("d1", new List<ResponseQuestionnaireDTO> {
            new ResponseQuestionnaireDTO { pergunta = "p", resposta = "r" }
        });
        list.Should().HaveCount(1);
        list[0].DeviceId.Should().Be("d1");
    }
}
