using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.QuestionOptionRepository;

public class WhenUpsertingQuestionOption
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.QuestionOptionRepository _sut;

    public WhenUpsertingQuestionOption() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Upsert_QuestionOption()
    {
        // Arrange
        Guid questionOptionId = Guid.NewGuid();
        Guid questionId = Guid.NewGuid();

        QuestionOption newQuestionOption = new()
        {
            Id = questionOptionId,
            QuestionId = questionId
        };

        var dbSet = new List<QuestionOption>() { newQuestionOption };

        _context.SetupGet(c => c.QuestionOptions).ReturnsDbSet(dbSet);

        // Act
        await _sut.UpsertAsync(dbSet);

        // Assert
        _context.Verify(c => c.QuestionOptions.AddAsync(newQuestionOption, default), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
    }
}
