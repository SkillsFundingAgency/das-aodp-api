using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Routes;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Routes;

public class ConfigureRoutingForQuestionCommandHandlerTests
{
    public Mock<IRouteRepository> _routeRepository = new Mock<IRouteRepository>();
    public Mock<IQuestionRepository> _questionRepository = new Mock<IQuestionRepository>();
    public ConfigureRoutingForQuestionCommandHandler _configureRoutingForQuestionCommandHandler;

    public ConfigureRoutingForQuestionCommandHandlerTests()
    {
        _configureRoutingForQuestionCommandHandler = new(_routeRepository.Object, _questionRepository.Object);
    }

    [Fact]
    public async Task Test_Configure_Routes_For_Questions()
    {
        var request = new ConfigureRoutingForQuestionCommand()
        {
            QuestionId = Guid.NewGuid(),
            Routes = new()
        };
        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.QuestionId)))
            .ReturnsAsync(true);
        _routeRepository.Setup(v => v.UpsertAsync(It.IsAny<List<Data.Entities.FormBuilder.Route>>()))
            .Returns(Task.CompletedTask);

        var result = await _configureRoutingForQuestionCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Configure_Routes_For_Question_Erros_When_Question_Locked()
    {
        var request = new ConfigureRoutingForQuestionCommand()
        {
            QuestionId = Guid.NewGuid(),
            Routes = new()
        };
        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.QuestionId)))
            .ReturnsAsync(false);

        var result = await _configureRoutingForQuestionCommandHandler.Handle(request, default);

        Assert.False(result.Success);
    }
}
