using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.SectionRepository;

public class WhenCreatingSection
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.SectionRepository _sut;

    private readonly Mock<IPageRepository> _pageRepository = new();
    public WhenCreatingSection() => _sut = new(_context.Object, _pageRepository.Object);

    [Fact]
    public async Task Then_Create_Section()
    {
        // Arrange
        Section newSection = new();
        var dbSet = new List<Section>();

        _context.SetupGet(c => c.Sections).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.Create(newSection);

        // Assert
        _context.Verify(c => c.Sections.AddAsync(newSection, default), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());

        Assert.True(result.Id != Guid.Empty);
    }
}
