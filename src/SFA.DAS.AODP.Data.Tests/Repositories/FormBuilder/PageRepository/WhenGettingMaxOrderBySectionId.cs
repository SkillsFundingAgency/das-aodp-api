using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.PageRepository;

public class WhenGettingMaxOrderBySectionId
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.PageRepository _sut;

    public WhenGettingMaxOrderBySectionId() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Getting_Max_Order_By_SectionId()
    {
        // Arrange
        Guid pageId = Guid.NewGuid();

        Page page = new()
        {
            Id = pageId,
            Order = 0
        };

        var dbSet = new List<Page>() { page };

        _context.SetupGet(c => c.Pages).ReturnsDbSet(dbSet);

        // Act
        var result = _sut.GetMaxOrderBySectionId(pageId);

        // Assert
        Assert.Equal(page.Order, result);
    }
}
