using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Question;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Questions;

public class DeleteQuestionCommandHandlerTests
{
    private readonly Mock<IQuestionRepository> _questionRepository = new Mock<IQuestionRepository>();
    public DeleteQuestionCommandHandler _deleteFormVersionCommandHandler;

    public DeleteQuestionCommandHandlerTests()
    {
        _deleteFormVersionCommandHandler = new(_questionRepository.Object);
    }

    [Fact]
    public async Task Test_Delete_Form_Version()
    {
        var request = new DeleteQuestionCommand(Guid.NewGuid());
        _questionRepository.Setup(v => v.IsQuestionEditable(It.Is<Guid>(v => v == request.QuestionId)))
            .Returns(Task.FromResult(true));
        _questionRepository.Setup(v => v.Archive(It.Is<Guid>(v => v == request.QuestionId)))
            .Returns(Task.FromResult(true));

        var result = await _deleteFormVersionCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Delete_Form_Throws_Returns_Error()
    {
        var request = new DeleteQuestionCommand(Guid.NewGuid());
        _questionRepository.Setup(v => v.IsQuestionEditable(It.Is<Guid>(v => v == request.QuestionId)))
            .Returns(Task.FromResult(true));
        _questionRepository.Setup(v => v.Archive(It.Is<Guid>(v => v == request.QuestionId)))
            .Throws(new Exception("Test"));

        var result = await _deleteFormVersionCommandHandler.Handle(request, default);

        Assert.False(result.Success);
        Assert.Equal("Test", result.ErrorMessage);
    }
}
