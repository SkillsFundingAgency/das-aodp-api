using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Question;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Questions;

public class MoveQuestionDownCommandHandlerTests
{
    private readonly Mock<IQuestionRepository> _questionRepository = new Mock<IQuestionRepository>();
    public MoveQuestionDownCommandHandler _moveQuestionDownCommandHandler;

    public MoveQuestionDownCommandHandlerTests()
    {
        _moveQuestionDownCommandHandler = new(_questionRepository.Object);
    }

    [Fact]
    public async Task Test_Move_Question_Order_Down()
    {
        var request = new MoveQuestionDownCommand
        {
            QuestionId = Guid.NewGuid()
        };
        _questionRepository.Setup(v => v.IsQuestionEditable(It.Is<Guid>(v => v == request.QuestionId)))
            .Returns(Task.FromResult(true));
        _questionRepository.Setup(v => v.MoveQuestionOrderDown(It.Is<Guid>(v => v == request.QuestionId)))
            .Returns(Task.FromResult(true));

        var result = await _moveQuestionDownCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Move_Question_Order_Down_Throws_Returns_Error()
    {
        var request = new MoveQuestionDownCommand
        {
            QuestionId = Guid.NewGuid()
        };
        _questionRepository.Setup(v => v.IsQuestionEditable(It.Is<Guid>(v => v == request.QuestionId)))
            .Returns(Task.FromResult(true));
        _questionRepository.Setup(v => v.MoveQuestionOrderDown(It.Is<Guid>(v => v == request.QuestionId)))
            .Throws(new Exception("Test"));

        var result = await _moveQuestionDownCommandHandler.Handle(request, default);

        Assert.False(result.Success);
        Assert.Equal("Test", result.ErrorMessage);
    }
}
