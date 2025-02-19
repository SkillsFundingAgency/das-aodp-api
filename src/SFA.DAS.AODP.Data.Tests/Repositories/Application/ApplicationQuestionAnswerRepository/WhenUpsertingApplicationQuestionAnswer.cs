using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationQuestionAnswerRepository;

public class WhenUpsertingApplicationQuestionAnswer
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.Application.ApplicationQuestionAnswerRepository _sut;

    public WhenUpsertingApplicationQuestionAnswer() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_ApplicationQuestionAnswer_Is_Upserted()
    {
        // Arrange
        Guid applicationId = Guid.NewGuid();
        Guid questionId = Guid.NewGuid();

        ApplicationQuestionAnswer questionAnswer = new()
        {
            Id = applicationId,
            QuestionId = questionId
        };

        var dbSet = new List<ApplicationQuestionAnswer>() { questionAnswer };

        _context.SetupGet(c => c.ApplicationQuestionAnswers).ReturnsDbSet(dbSet);

        // Act
        await _sut.UpsertAsync(dbSet);

        // Assert
        _context.Verify(c => c.ApplicationQuestionAnswers.AddAsync(questionAnswer, default), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
    }
}



