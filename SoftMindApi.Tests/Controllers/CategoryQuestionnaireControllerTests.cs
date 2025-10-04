using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SoftMindApi.Controllers;
using SoftMindApi.DTO;
using SoftMindApi.Entities;
using SoftMindApi.Services.Interface;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;

namespace SoftMindApi.Tests.Controllers;

public class CategoryQuestionnaireControllerTests
{
    [Fact]
    public async Task AddResponseQuestionnaire_ShouldReturnBadRequest_WhenInvalidArgs()
    {
        var ctrl = new CategoryQuestionnaireController(Mock.Of<ICategoryQuestionnaireService>());
        var res = await ctrl.PostResponseQuestionnaire("", new List<ResponseQuestionnaireDTO>());
        res.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetCategoryQuestionnaire_ShouldReturnMessage_WhenEmpty()
    {
        var svc = new Mock<ICategoryQuestionnaireService>();
        svc.Setup(s => s.GetCategoriesAsync()).ReturnsAsync(new List<CategoryQuestionnaire>());
        var ctrl = new CategoryQuestionnaireController(svc.Object);
        var res = await ctrl.GetCategoryQuestionnaire() as OkObjectResult;
        res.Should().NotBeNull();
    }
}
