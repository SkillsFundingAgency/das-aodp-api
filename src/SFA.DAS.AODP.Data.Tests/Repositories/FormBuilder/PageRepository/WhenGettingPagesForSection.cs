using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.PageRepository;

public class WhenGettingPagesForSection
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.PageRepository _sut;

    public WhenGettingPagesForSection() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_Pages_For_Section()
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

        var dbSet = new List<Page>() { newPage1, newPage2 };

        _context.SetupGet(c => c.Pages).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.GetPagesForSectionAsync(sectionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dbSet, result);
    }
}
