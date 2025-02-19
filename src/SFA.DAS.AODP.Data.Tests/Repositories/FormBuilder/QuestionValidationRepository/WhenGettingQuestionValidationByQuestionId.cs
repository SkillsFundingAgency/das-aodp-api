using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.QuestionValidationRepository;

public class WhenGettingQuestionValidationByQuestionId
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.QuestionValidationRepository _sut;

    public WhenGettingQuestionValidationByQuestionId() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_Question_Validation_By_Question_Id()
    {
        Guid questionValidationId = Guid.NewGuid();
        Guid questionId = Guid.NewGuid();

        QuestionValidation newQuestionValidation = new()
        {
            Id = questionValidationId,
            QuestionId = questionId
        };

        var dbSet = new List<QuestionValidation>() { newQuestionValidation };

        _context.SetupGet(c => c.QuestionValidations).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.GetQuestionValidationsByQuestionIdAsync(questionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newQuestionValidation, result);
    }
}
