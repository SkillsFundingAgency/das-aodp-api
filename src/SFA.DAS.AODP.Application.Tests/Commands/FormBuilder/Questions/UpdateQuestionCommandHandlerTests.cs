using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Question;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Questions;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Models.Settings;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Questions;

public class UpdateQuestionCommandHandlerTests
{
    public Mock<IQuestionRepository> _questionRepository = new Mock<IQuestionRepository>();
    public Mock<IQuestionValidationRepository> _questionValidationRepository = new Mock<IQuestionValidationRepository>();
    public Mock<IQuestionOptionRepository> _questionOptionRepositor = new Mock<IQuestionOptionRepository>();
    public FormBuilderSettings _formBuilderSettings = new();
    public UpdateQuestionCommandHandler _updateQuestionCommandHandler;

    public UpdateQuestionCommandHandlerTests()
    {
        _updateQuestionCommandHandler = new(_questionRepository.Object, _questionValidationRepository.Object, _questionOptionRepositor.Object, _formBuilderSettings);
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
            Id = Guid.NewGuid(),
            Title = "",
            Hint = "",
        };

        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.Id)))
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

        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.Id)))
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

    [Fact]
    public async Task Test_Create_Question_Throws_When_File_Number_Is_Greater_Than_Allowed()
    {
        // Arrange
        var request = new UpdateQuestionCommand()
        {
            Id = Guid.NewGuid(),
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            Title = "Test",
            FileUpload = new()
            {
                NumberOfFiles = 2
            }
        };

        _formBuilderSettings.MaxUploadNumberOfFiles = 1;

        var newQuestion = new Question() { Id = Guid.NewGuid(), Type = QuestionType.File.ToString() };

        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(true);

        _questionRepository.Setup(v => v.GetQuestionByIdAsync(It.Is<Guid>(v => v == request.Id)))
           .ReturnsAsync(newQuestion);

        // Act
        var result = await _updateQuestionCommandHandler.Handle(request, default);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.InnerException);
        Assert.Contains("Max number of files allowed", result.InnerException.Message);
    }

    [Fact]
    public async Task Test_Create_Question_Throws_Text_Length_On_Failure()
    {
        // Arrange
        var request = new UpdateQuestionCommand()
        {
            Id = Guid.NewGuid(),
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            Title = "Test",
            TextInput = new()
            {
                MinLength = 2,
                MaxLength = 1
            }
        };

        var newQuestion = new Question() { Id = Guid.NewGuid(), Type = QuestionType.Text.ToString() };

        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(true);

        _questionRepository.Setup(v => v.GetQuestionByIdAsync(It.Is<Guid>(v => v == request.Id)))
           .ReturnsAsync(newQuestion);

        // Act
        var result = await _updateQuestionCommandHandler.Handle(request, default);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.InnerException);
        Assert.Contains("The minimum length cannot be greater than", result.InnerException.Message);
        Assert.Contains("The maximum length cannot be less than", result.InnerException.Message);
    }

    [Fact]
    public async Task Test_Create_Question_Throws_Number_Is_Greater_And_Less_Than_Or_Equal_To_Than_Allowed()
    {
        // Arrange
        var request = new UpdateQuestionCommand()
        {
            Id = Guid.NewGuid(),
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            Title = "Test",
            NumberInput = new()
            {
                LessThanOrEqualTo = 2,
                GreaterThanOrEqualTo = 1
            }
        };

        var newQuestion = new Question() { Id = Guid.NewGuid(), Type = QuestionType.Number.ToString() };

        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(true);

        _questionRepository.Setup(v => v.GetQuestionByIdAsync(It.Is<Guid>(v => v == request.Id)))
           .ReturnsAsync(newQuestion);

        // Act
        var result = await _updateQuestionCommandHandler.Handle(request, default);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.InnerException);
        Assert.Contains("The number provided cannot be greater than or equal to", result.InnerException.Message);
        Assert.Contains("The number provided cannot be less than or equal to", result.InnerException.Message);
    }

    [Fact]
    public async Task Test_Create_Question_Throws_Number_Is_Not_Equal_And_Less_Than_Or_Equal_To_Than_Allowed()
    {
        // Arrange
        var request = new UpdateQuestionCommand()
        {
            Id = Guid.NewGuid(),
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            Title = "Test",
            NumberInput = new()
            {
                LessThanOrEqualTo = 2,
                NotEqualTo = 1
            }
        };

        var newQuestion = new Question() { Id = Guid.NewGuid(), Type = QuestionType.Number.ToString() };

        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(true);

        _questionRepository.Setup(v => v.GetQuestionByIdAsync(It.Is<Guid>(v => v == request.Id)))
           .ReturnsAsync(newQuestion);

        // Act
        var result = await _updateQuestionCommandHandler.Handle(request, default);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.InnerException);
        Assert.Contains("The not allowed value/s provided cannot be less than or equal to", result.InnerException.Message);
    }

    [Fact]
    public async Task Test_Create_Question_Throws_Number_Is_Not_Equal_And_Greater_Than_Or_Equal_To_Than_Allowed()
    {
        // Arrange
        var request = new UpdateQuestionCommand()
        {
            Id = Guid.NewGuid(),
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            Title = "Test",
            NumberInput = new()
            {
                GreaterThanOrEqualTo = 1,
                NotEqualTo = 2
            }
        };

        var newQuestion = new Question() { Id = Guid.NewGuid(), Type = QuestionType.Number.ToString() };

        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(true);

        _questionRepository.Setup(v => v.GetQuestionByIdAsync(It.Is<Guid>(v => v == request.Id)))
           .ReturnsAsync(newQuestion);

        // Act
        var result = await _updateQuestionCommandHandler.Handle(request, default);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.InnerException);
        Assert.Contains("The not allowed value/s provided cannot be greater than or equal to", result.InnerException.Message);
    }

    [Fact]
    public async Task Test_Create_Question_Throws_Checkbox_Minimum_Number_Of_Options_Is_Negative()
    {
        // Arrange
        var request = new UpdateQuestionCommand()
        {
            Id = Guid.NewGuid(),
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            Title = "Test",
            Checkbox = new()
            {
                MinNumberOfOptions = -1,
            }
        };

        var newQuestion = new Question() { Id = Guid.NewGuid(), Type = QuestionType.MultiChoice.ToString() };

        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(true);

        _questionRepository.Setup(v => v.GetQuestionByIdAsync(It.Is<Guid>(v => v == request.Id)))
           .ReturnsAsync(newQuestion);

        // Act
        var result = await _updateQuestionCommandHandler.Handle(request, default);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.InnerException);
        Assert.Contains("The minimum number of checkbox options must not be a negative number", result.InnerException.Message);
    }

    [Fact]
    public async Task Test_Create_Question_Throws_Checkbox_Maximum_Number_Of_Options_Is_Negative()
    {
        // Arrange
        var request = new UpdateQuestionCommand()
        {
            Id = Guid.NewGuid(),
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            Title = "Test",
            Checkbox = new()
            {
                MaxNumberOfOptions = -1,
            }
        };

        var newQuestion = new Question() { Id = Guid.NewGuid(), Type = QuestionType.MultiChoice.ToString() };

        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(true);

        _questionRepository.Setup(v => v.GetQuestionByIdAsync(It.Is<Guid>(v => v == request.Id)))
           .ReturnsAsync(newQuestion);

        // Act
        var result = await _updateQuestionCommandHandler.Handle(request, default);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.InnerException);
        Assert.Contains("The maximum number of checkbox options must not be a negative number", result.InnerException.Message);
    }

    [Fact]
    public async Task Test_Create_Question_Throws_Checkbox_Minimum_Number_Of_Options_Is_Greater_Than_Maximum()
    {
        // Arrange
        var request = new UpdateQuestionCommand()
        {
            Id = Guid.NewGuid(),
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            Title = "Test",
            Checkbox = new()
            {
                MinNumberOfOptions = 2,
                MaxNumberOfOptions = 1
            }
        };

        var newQuestion = new Question() { Id = Guid.NewGuid(), Type = QuestionType.MultiChoice.ToString() };

        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(true);

        _questionRepository.Setup(v => v.GetQuestionByIdAsync(It.Is<Guid>(v => v == request.Id)))
           .ReturnsAsync(newQuestion);

        // Act
        var result = await _updateQuestionCommandHandler.Handle(request, default);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.InnerException);
        Assert.Contains("The minimum number of checkbox options must be less than", result.InnerException.Message);
        Assert.Contains("The maximum number of checkbox options must be greater than", result.InnerException.Message);
    }

    [Fact]
    public async Task Test_Create_Question_Throws_Checkbox_Minimum_Number_Of_Options_Is_Greater_Than_Number_Of_Options()
    {
        // Arrange
        var request = new UpdateQuestionCommand()
        {
            Id = Guid.NewGuid(),
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            Title = "Test",
            Options = new()
            {
                new()
            },
            Checkbox = new()
            {
                MinNumberOfOptions = 2,
            }
        };

        var newQuestion = new Question() { Id = Guid.NewGuid(), Type = QuestionType.MultiChoice.ToString() };

        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(true);

        _questionRepository.Setup(v => v.GetQuestionByIdAsync(It.Is<Guid>(v => v == request.Id)))
           .ReturnsAsync(newQuestion);

        // Act
        var result = await _updateQuestionCommandHandler.Handle(request, default);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.InnerException);
        Assert.Contains("The number of checkbox options cannot be less than", result.InnerException.Message);
    }

    [Fact]
    public async Task Test_Create_Question_Throws_Checkbox_Maximum_Number_Of_Options_Is_Less_Than_Number_Of_Options()
    {
        // Arrange
        var request = new UpdateQuestionCommand()
        {
            Id = Guid.NewGuid(),
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            Title = "Test",
            Options = new()
            {
                new(),
                new()
            },
            Checkbox = new()
            {
                MaxNumberOfOptions = 1,
            }
        };

        var newQuestion = new Question() { Id = Guid.NewGuid(), Type = QuestionType.MultiChoice.ToString() };

        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(true);

        _questionRepository.Setup(v => v.GetQuestionByIdAsync(It.Is<Guid>(v => v == request.Id)))
           .ReturnsAsync(newQuestion);

        // Act
        var result = await _updateQuestionCommandHandler.Handle(request, default);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.InnerException);
        Assert.Contains("The number of checkbox options cannot be greater than", result.InnerException.Message);
    }

    [Fact]
    public async Task Test_Create_Question_Throws_Date_Is_Greater_And_Less_Than_Or_Equal_To_Than_Allowed()
    {
        // Arrange
        var request = new UpdateQuestionCommand()
        {
            Id = Guid.NewGuid(),
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            Title = "Test",
            DateInput = new()
            {
                LessThanOrEqualTo = DateOnly.MaxValue,
                GreaterThanOrEqualTo = DateOnly.MinValue
            }
        };

        var newQuestion = new Question() { Id = Guid.NewGuid(), Type = QuestionType.Date.ToString() };

        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(true);

        _questionRepository.Setup(v => v.GetQuestionByIdAsync(It.Is<Guid>(v => v == request.Id)))
           .ReturnsAsync(newQuestion);

        // Act
        var result = await _updateQuestionCommandHandler.Handle(request, default);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.InnerException);
        Assert.Contains("The date provided must be earlier than", result.InnerException.Message);
        Assert.Contains("The date provided must be later than", result.InnerException.Message);
    }

    [Fact]
    public async Task Test_Create_Question_Throws_InvalidType()
    {
        // Arrange
        var request = new UpdateQuestionCommand()
        {
            Id = Guid.NewGuid(),
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            Title = "Test",
        };

        var newQuestion = new Question() { Id = Guid.NewGuid(), Type = null };

        _questionRepository.Setup(v => v.IsQuestionEditableAsync(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(true);

        _questionRepository.Setup(v => v.GetQuestionByIdAsync(It.Is<Guid>(v => v == request.Id)))
           .ReturnsAsync(newQuestion);

        // Act
        var result = await _updateQuestionCommandHandler.Handle(request, default);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.InnerException);
        Assert.Contains("The type provided must be valid.", result.InnerException.Message);
    }
}
