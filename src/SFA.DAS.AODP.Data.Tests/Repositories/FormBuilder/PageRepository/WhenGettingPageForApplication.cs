using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.PageRepository;

public class WhenGettingPageForApplication
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.PageRepository _sut;

    public WhenGettingPageForApplication() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_Page_For_Application()
    {
        // Arrange
        Guid sectionId = Guid.NewGuid();
        Guid pageId = Guid.NewGuid();

        Page newPage = new()
        {
            Id = pageId,
        };

        var dbSet = new List<Page>() { newPage };

        _context.SetupGet(c => c.Pages).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.GetPageForApplicationAsync(0, sectionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newPage, result);
    }
}
