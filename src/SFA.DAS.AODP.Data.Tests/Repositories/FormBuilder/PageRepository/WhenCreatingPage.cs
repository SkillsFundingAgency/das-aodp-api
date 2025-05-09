using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.PageRepository;

public class WhenCreatingPage
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.PageRepository _sut;

    public WhenCreatingPage() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_The_Page_Is_Created()
    {
        // Arrange
        Page newPage = new() { SectionId = Guid.NewGuid() };
        Section newSection = new() { Id = newPage.SectionId };
        var dbSet = new List<Page>();
        var sections = new List<Section>() { newSection };

        _context.SetupGet(c => c.Pages).ReturnsDbSet(dbSet);
        _context.SetupGet(c => c.Sections).ReturnsDbSet(sections);

        // Act
        var result = await _sut.Create(newPage);

        // Assert
        _context.Verify(c => c.Pages.Add(newPage), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());

        Assert.True(result.Id != Guid.Empty);
    }
}