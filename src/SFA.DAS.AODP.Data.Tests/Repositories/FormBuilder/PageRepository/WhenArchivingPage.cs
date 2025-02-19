using Azure;
using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Models.Form;

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
        Section section = new()
        {
            Id = Guid.NewGuid(),
            FormVersion = new()
            {
                Status = FormVersionStatus.Draft.ToString(),
            }
        };

        var dbSet = new List<Page>() { page };
        var dbSetSections = new List<Section>() { section };

        _context.SetupGet(c => c.Pages).ReturnsDbSet(dbSet);
        _context.SetupGet(c => c.Sections).ReturnsDbSet(dbSetSections);

        // Act
        var result = await _sut.Archive(pageId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(page, result);
    }
}