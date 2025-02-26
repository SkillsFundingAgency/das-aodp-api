using Azure.Core;
using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.QuestionRepository;

public class WhenArchivingQuestion
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.QuestionRepository _sut;

    public WhenArchivingQuestion() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Archive_Question()
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
        var result = _sut.Archive(questionId);

        // Assert
        Assert.NotNull(result);
        // Assert.Equal(newQuestion, result.Status);


        // if (!await IsQuestionEditable(questionId)) throw new RecordLockedException();

        //         var question = await GetQuestionByIdAsync(questionId);

        // _context.Questions.Remove(question);

        // await _context.SaveChangesAsync();
    }
}
