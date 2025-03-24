using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Questions;

public class MoveQuestionUpCommandHandlerTests
{
    private readonly Mock<IQuestionRepository> _questionRepository = new Mock<IQuestionRepository>();
    public MoveQuestionUpCommandHandler _moveQuestionUpCommandHandler;

    public MoveQuestionUpCommandHandlerTests()
    {
        _moveQuestionUpCommandHandler = new(_questionRepository.Object);
    }

    [Fact]
    public async Task Test_Move_Question_Order_Down()
    {
        var request = new MoveQuestionUpCommand
        {
            QuestionId = Guid.NewGuid()
        };
        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.QuestionId)))
            .Returns(Task.FromResult(true));
        _questionRepository.Setup(v => v.MoveQuestionOrderUp(It.Is<Guid>(v => v == request.QuestionId)))
            .Returns(Task.FromResult(true));

        var result = await _moveQuestionUpCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Move_Question_Order_Down_Throws_Returns_Error()
    {
        var request = new MoveQuestionUpCommand
        {
            QuestionId = Guid.NewGuid()
        };
        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.QuestionId)))
            .Returns(Task.FromResult(true));
        _questionRepository.Setup(v => v.MoveQuestionOrderUp(It.Is<Guid>(v => v == request.QuestionId)))
            .Throws(new Exception("Test"));

        var result = await _moveQuestionUpCommandHandler.Handle(request, default);

        Assert.False(result.Success);
        Assert.Equal("Test", result.ErrorMessage);
    }
}
