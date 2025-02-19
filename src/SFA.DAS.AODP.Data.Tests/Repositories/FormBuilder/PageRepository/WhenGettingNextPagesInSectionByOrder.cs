using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.PageRepository;

public class WhenGettingNextPagesInSectionByOrder
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.PageRepository _sut;

    public WhenGettingNextPagesInSectionByOrder()
    {
        _sut = new(_context.Object);
    }

    [Fact]
    public async Task Then_Get_Next_Pages_In_Section_By_Order()
    {
        // Arrange
        Guid sectionId = Guid.NewGuid();
        Guid pageId1 = Guid.NewGuid();
        Guid pageId2 = Guid.NewGuid();

        Page newPage1 = new()
        {
            Id = pageId1,
            SectionId = sectionId,
            Order = 0
        };

        Page newPage2 = new()
        {
            Id = pageId2,
            SectionId = sectionId,
            Order = 1
        };

        Section newSection = new()
        {
            Id = sectionId,
            Pages = [
                newPage1,
                newPage2
            ]
        };

        var solution = new List<Page>() { newPage2 };

        var dbSet = new List<Section>() { newSection };
        var pages = new List<Page>() { newPage1, newPage2 };

        _context.SetupGet(c => c.Sections).ReturnsDbSet(dbSet);
        _context.SetupGet(c => c.Pages).ReturnsDbSet(pages);

        // Act
        var result = await _sut.GetNextPagesInSectionByOrderAsync(sectionId, 0);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(solution, result);
    }
}
