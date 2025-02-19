using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.QuestionValidationRepository;

public class WhenCopyingQuestionValidationForNewFormVersion
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.QuestionValidationRepository _sut;

    public WhenCopyingQuestionValidationForNewFormVersion() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Copy_Question_Validation_For_New_FormVersion()
    {
        // Arrange
        Guid migrateFromQuestionId = Guid.NewGuid();
        Guid migrateToQuestionId = Guid.NewGuid();
        Guid questionValidationId = Guid.NewGuid();

        QuestionValidation newQuestionValidation = new()
        {
            Id = questionValidationId,
            QuestionId = migrateFromQuestionId
        };

        var dbSet = new List<QuestionValidation>() { newQuestionValidation };

        _context.SetupGet(c => c.QuestionValidations).ReturnsDbSet(dbSet);

        Dictionary<Guid,Guid> oldNewQuestionsIds = new Dictionary<Guid, Guid>
        {
            {migrateFromQuestionId, migrateToQuestionId}
        };

        // Act
        await _sut.CopyQuestionValidationForNewFormVersion(oldNewQuestionsIds);

        // Assert
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
        // Assert.NotNull(result);
        // Assert.Contains(questionValidationId, result.Keys);

        //         var oldIds = oldNewQuestionIds.Keys.ToList();

        // var toMigrate = await _context.QuestionValidations.AsNoTracking().Where(v => oldIds.Contains(v.QuestionId)).ToListAsync();
        // foreach (var entity in toMigrate)
        // {
        //     entity.QuestionId = oldNewQuestionIds[entity.QuestionId];
        //     entity.Id = Guid.NewGuid();
        // }
        // await _context.QuestionValidations.AddRangeAsync(toMigrate);
        // await _context.SaveChangesAsync();
    }
}
