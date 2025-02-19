using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationQuestionAnswerRepository;

public class WhenGettingApplicationQuestionAnswerByApplicationIdAndPageId
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.Application.ApplicationQuestionAnswerRepository _sut;

    public WhenGettingApplicationQuestionAnswerByApplicationIdAndPageId() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_ApplicationQuestionAnswer_By_ApplicationIdAndPageId()
    {
        // Arrange
        Guid applicationId = Guid.NewGuid();
        Guid pageId = Guid.NewGuid();

        ApplicationQuestionAnswer questionAnswer = new()
        {
            ApplicationPageId = pageId,
            Id = applicationId
        };

        var dbSet = new List<ApplicationQuestionAnswer>() { questionAnswer };

        _context.SetupGet(c => c.ApplicationQuestionAnswers).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.GetAnswersByApplicationAndPageId(applicationId, pageId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dbSet, result);
    }
}



