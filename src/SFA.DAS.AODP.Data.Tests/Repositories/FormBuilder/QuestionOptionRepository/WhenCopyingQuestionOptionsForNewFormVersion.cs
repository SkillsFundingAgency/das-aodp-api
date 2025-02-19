using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.QuestionOptionRepository;

public class WhenCopyingQuestionOptionsForNewFormVersion
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.QuestionOptionRepository _sut;

    public WhenCopyingQuestionOptionsForNewFormVersion() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Copy_Question_Options_For_New_Form_Version()
    {
        // Arrange
        Guid migrateFromQuestionId = Guid.NewGuid();
        Guid migrateToQuestionId = Guid.NewGuid();
        Guid questionOptionId = Guid.NewGuid();

        QuestionOption newQuestionOption = new()
        {
            Id = questionOptionId,
            QuestionId = migrateFromQuestionId
        };

        var dbSet = new List<QuestionOption>() { newQuestionOption };

        _context.SetupGet(c => c.QuestionOptions).ReturnsDbSet(dbSet);

        Dictionary<Guid,Guid> oldNewQuestionIds = new Dictionary<Guid, Guid>
        {
            {migrateFromQuestionId, migrateToQuestionId}
        };

        // Act
        var result = await _sut.CopyQuestionOptionsForNewFormVersion(oldNewQuestionIds);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(questionOptionId, result.Keys);
    }
}
