using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.PageRepository;

public class WhenCopyingPagesForNewFormVersion
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.PageRepository _sut;

    public WhenCopyingPagesForNewFormVersion() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Copy_Page_For_New_FormVersion()
    {
        // Arrange
        Guid migrateFromSectionId = Guid.NewGuid();
        Guid migrateToSectionId = Guid.NewGuid();
        Guid pageId = Guid.NewGuid();

        Page newPage = new()
        {
            Id = pageId,
            SectionId = migrateFromSectionId
        };

        var dbSet = new List<Page>() { newPage };

        _context.SetupGet(c => c.Pages).ReturnsDbSet(dbSet);

        Dictionary<Guid,Guid> oldNewSectionIds = new Dictionary<Guid, Guid>
        {
            {migrateFromSectionId, migrateToSectionId}
        };

        // Act
        var result = await _sut.CopyPagesForNewFormVersion(oldNewSectionIds);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(pageId, result.Keys);
    }
}
