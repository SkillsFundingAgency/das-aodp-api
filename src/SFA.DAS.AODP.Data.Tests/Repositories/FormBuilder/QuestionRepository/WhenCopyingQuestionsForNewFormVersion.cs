using Azure.Core;
using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.QuestionRepository;

public class WhenCopyingQuestionsForNewFormVersion
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.QuestionRepository _sut;

    public WhenCopyingQuestionsForNewFormVersion() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Copy_Questions_For_New_FormVersion()
    {
        // Arrange
        Guid migrateFromPageId = Guid.NewGuid();
        Guid migrateToPageId = Guid.NewGuid();
        Guid questionId = Guid.NewGuid();

        Question newQuestion = new()
        {
            Id = questionId,
            PageId = migrateFromPageId
        };

        var dbSet = new List<Question>() { newQuestion };

        _context.SetupGet(c => c.Questions).ReturnsDbSet(dbSet);

        Dictionary<Guid,Guid> oldNewPageIds = new Dictionary<Guid, Guid>
        {
            {migrateFromPageId, migrateToPageId}
        };

        // Act
        var result = await _sut.CopyQuestionsForNewFormVersion(oldNewPageIds);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(questionId, result.Keys);
    }
}
