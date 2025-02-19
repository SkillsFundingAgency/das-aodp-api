using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.QuestionRepository;

public class WhenGettingQuestionTypeById
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.QuestionRepository _sut;

    public WhenGettingQuestionTypeById() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_Question_By_Id()
    {
        Guid questionId = Guid.NewGuid();

        Question newQuestion = new()
        {
            Id = questionId,
            Type = "1" // TextArea value from enum
        };

        var dbSet = new List<Question>() { newQuestion };

        _context.SetupGet(c => c.Questions).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.GetQuestionTypeById(questionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newQuestion.Type, result);
    }
}
