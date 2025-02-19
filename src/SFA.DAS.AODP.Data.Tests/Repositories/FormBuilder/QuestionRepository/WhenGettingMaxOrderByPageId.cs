using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.QuestionRepository;

public class WhenGettingMaxOrderByPageId
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.QuestionRepository _sut;

    public WhenGettingMaxOrderByPageId() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_Max_Order_By_Page_Id()
    {
        // Arrange
        Guid pageId = Guid.NewGuid();
        Guid questionId = Guid.NewGuid();

        Page page = new()
        {
            Id = pageId,
            Order = 0,
            Questions = [
                new Question()
                {
                    Id = questionId
                }
            ]
        };

        var dbSet = new List<Page>() { page };

        _context.SetupGet(c => c.Pages).ReturnsDbSet(dbSet);

        // Act
        var result = _sut.GetMaxOrderByPageId(pageId);

        // Assert
        Assert.Equal(page.Order, result);
    }
}
