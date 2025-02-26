﻿using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Question;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Questions;

public class UpdateQuestionCommandHandlerTests
{
    public Mock<IQuestionRepository> _questionRepository = new Mock<IQuestionRepository>();
    public Mock<IQuestionValidationRepository> _questionValidationRepository = new Mock<IQuestionValidationRepository>();
    public Mock<IQuestionOptionRepository> _questionOptionRepositor = new Mock<IQuestionOptionRepository>();
    public UpdateQuestionCommandHandler _updateQuestionCommandHandler;

    public UpdateQuestionCommandHandlerTests()
    {
        _updateQuestionCommandHandler = new(_questionRepository.Object, _questionValidationRepository.Object, _questionOptionRepositor.Object);
    }

    [Fact]
    public async Task Test_Create_Question()
    {
        var request = new UpdateQuestionCommand()
        {
            Id = Guid.NewGuid(),
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            Title = "Test",
            Hint = "Test",
            Required = true,
        };

        var newQuestion = new Question() 
        { 
            Id = Guid.NewGuid() ,
            Title = "",
            Hint = "",
        };

        _questionRepository.Setup(v => v.IsQuestionEditable(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(true);

        _questionRepository.Setup(v => v.GetQuestionByIdAsync(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(newQuestion);

        _questionRepository.Setup(v => v.Create(It.Is<Question>(v =>
            v.Required == request.Required && v.Title == request.Title &&
            v.Order == 4 && v.PageId == request.PageId)))
            .ReturnsAsync(newQuestion);

        var result = await _updateQuestionCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Create_Question_Throws_When_Page_Locked()
    {
        var request = new UpdateQuestionCommand()
        {
            Id = Guid.NewGuid(),
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            Title = "Test"
        };
        var newQuestion = new Question() { Id = Guid.NewGuid() };

        _questionRepository.Setup(v => v.IsQuestionEditable(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(false);

        _questionRepository.Setup(v => v.GetMaxOrderByPageId(It.Is<Guid>(v => v == request.PageId)))
            .Returns(3);

        _questionRepository.Setup(v => v.Create(It.Is<Question>(v =>
            v.Required == request.Required && v.Title == request.Title &&
            v.Order == 4 && v.PageId == request.PageId)))
            .ReturnsAsync(newQuestion);

        var result = await _updateQuestionCommandHandler.Handle(request, default);

        Assert.False(result.Success);
    }
}
