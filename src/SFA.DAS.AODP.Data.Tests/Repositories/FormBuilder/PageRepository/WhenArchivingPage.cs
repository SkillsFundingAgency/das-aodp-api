using Azure;
using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.PageRepository;

public class WhenArchivingPage
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.PageRepository _sut;

    public WhenArchivingPage() => _sut = new(_context.Object);

    [Fact]
    public async Task When_The_Page_Is_Archived()
    {  
        // Arrange
        Guid pageId = Guid.NewGuid();

        Page page = new()
        {
            Id = pageId,
            // No status?
        };

        var dbSet = new List<Page>() { page };

        _context.SetupGet(c => c.Pages).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.Archive(pageId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(page, result);
    }
}