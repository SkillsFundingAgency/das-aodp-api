using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Question;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Questions;

public class CreateQuestionCommandHandlerTests
{
    private readonly Mock<IQuestionRepository> _questionRepository = new Mock<IQuestionRepository>();
    private readonly Mock<IPageRepository> _pageRepository = new Mock<IPageRepository>();
    private readonly Mock<IQuestionValidationRepository> _questionValidationRepository = new Mock<IQuestionValidationRepository>();
    public CreateQuestionCommandHandler _createQuestionCommandHandler;

    public CreateQuestionCommandHandlerTests()
    {
        _createQuestionCommandHandler = new(_questionRepository.Object, _pageRepository.Object, _questionValidationRepository.Object);
    }

    [Fact]
    public async Task Test_Create_Question()
    {
        var request = new CreateQuestionCommand()
        {
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            Title = "Test",
            Type = QuestionType.Text.ToString(),
        };
        var newQuestion = new Question() { Id = Guid.NewGuid() };

        _pageRepository.Setup(v => v.IsPageEditable(It.Is<Guid>(v => v == request.PageId)))
            .ReturnsAsync(true);

        _questionRepository.Setup(v => v.GetMaxOrderByPageId(It.Is<Guid>(v => v == request.PageId)))
            .Returns(3);

        _questionRepository.Setup(v => v.Create(It.Is<Question>(v =>
            v.Required == request.Required && v.Title == request.Title &&
            v.Order == 4 && v.PageId == request.PageId && v.Type == request.Type)))
            .ReturnsAsync(newQuestion);

        var result = await _createQuestionCommandHandler.Handle(request, default);

        Assert.True(result.Success);
        Assert.True(newQuestion.Id == result.Value.Id);
    }

    [Fact]
    public async Task Test_Create_Question_Inits_Validation_For_File()
    {
        var request = new CreateQuestionCommand()
        {
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            Title = "Test",
            Type = QuestionType.File.ToString(),
        };
        var newQuestion = new Question() { Id = Guid.NewGuid(), Type = QuestionType.File.ToString() };

        _pageRepository.Setup(v => v.IsPageEditable(It.Is<Guid>(v => v == request.PageId)))
            .ReturnsAsync(true);

        _questionRepository.Setup(v => v.GetMaxOrderByPageId(It.Is<Guid>(v => v == request.PageId)))
            .Returns(3);

        _questionRepository.Setup(v => v.Create(It.Is<Question>(v =>
            v.Required == request.Required && v.Title == request.Title &&
            v.Order == 4 && v.PageId == request.PageId && v.Type == request.Type)))
            .ReturnsAsync(newQuestion);

        var result = await _createQuestionCommandHandler.Handle(request, default);

        _questionValidationRepository.Verify(q => q.UpsertAsync(It.Is<QuestionValidation>(q => q.NumberOfFiles == 1)));
        _questionValidationRepository.Verify(q => q.UpsertAsync(It.Is<QuestionValidation>(q => q.QuestionId == newQuestion.Id)));
    }

    [Fact]
    public async Task Test_Create_Question_Throws_When_Page_Locked()
    {
        var request = new CreateQuestionCommand()
        {
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            Title = "Test",
            Type = QuestionType.Text.ToString(),
        };
        var newQuestion = new Question() { Id = Guid.NewGuid() };

        _pageRepository.Setup(v => v.IsPageEditable(It.Is<Guid>(v => v == request.PageId)))
            .ReturnsAsync(false);
        var result = await _createQuestionCommandHandler.Handle(request, default);

        Assert.False(result.Success);
    }
}
