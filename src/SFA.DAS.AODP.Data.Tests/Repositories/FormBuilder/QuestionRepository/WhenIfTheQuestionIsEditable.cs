using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.QuestionRepository;

public class WhenIfTheQuestionIsEditable
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.QuestionRepository _sut;

    public WhenIfTheQuestionIsEditable() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_If_Question_Is_Editable()
    {
        // Arrange
        Guid questionId = Guid.NewGuid();

        Question newQuestion = new()
        {
            Id = questionId,
            
        };

        var dbSet = new List<Question>() { newQuestion };

        _context.SetupGet(c => c.Questions).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.IsQuestionEditable(questionId);

        // Assert
        Assert.True(result);
    }
}
