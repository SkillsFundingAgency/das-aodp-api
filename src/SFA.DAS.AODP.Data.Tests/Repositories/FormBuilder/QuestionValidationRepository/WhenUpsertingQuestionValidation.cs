using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.QuestionValidationRepository;

public class WhenUpsertingQuestionValidation
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.QuestionValidationRepository _sut;

    public WhenUpsertingQuestionValidation() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Upsert_Question_Validation()
    {
        // Arrange
        Guid questionValidationId = Guid.NewGuid();
        Guid questionId = Guid.NewGuid();

        QuestionValidation newQuestionValidation = new()
        {
            //Id = questionValidationId,
            QuestionId = questionId
        };

        var dbSet = new List<QuestionValidation>() { newQuestionValidation };

        _context.SetupGet(c => c.QuestionValidations).ReturnsDbSet(dbSet);

        // Act
        await _sut.UpsertAsync(newQuestionValidation);

        // Assert
        _context.Verify(c => c.QuestionValidations.Add(newQuestionValidation), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
    }
}
