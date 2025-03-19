using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Routes;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Routes;

public class DeleteRouteCommandHandlerTests
{
    public Mock<IRouteRepository> _routeRepository = new Mock<IRouteRepository>();
    public Mock<IQuestionRepository> _questionRepository = new Mock<IQuestionRepository>();
    public DeleteRouteCommandHandler _deleteRouteCommandHandler;

    public DeleteRouteCommandHandlerTests()
    {
        _deleteRouteCommandHandler = new(_routeRepository.Object, _questionRepository.Object);
    }

    [Fact]
    public async Task Test_Deletes_Routes_For_Questions()
    {
        var request = new DeleteRouteCommand()
        {
            QuestionId = Guid.NewGuid(),
        };
        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.QuestionId)))
            .ReturnsAsync(true);
        _routeRepository.Setup(v => v.DeleteRouteByQuestionIdAsync(request.QuestionId))
            .Returns(Task.CompletedTask);

        var result = await _deleteRouteCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Delete_Routes_For_Question_Errors_When_Question_Locked()
    {
        var request = new DeleteRouteCommand()
        {
            QuestionId = Guid.NewGuid(),
        };
        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.QuestionId)))
            .ReturnsAsync(false);

        var result = await _deleteRouteCommandHandler.Handle(request, default);

        Assert.False(result.Success);
    }
}