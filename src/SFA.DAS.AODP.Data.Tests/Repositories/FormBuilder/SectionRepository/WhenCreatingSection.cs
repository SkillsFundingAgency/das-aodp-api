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
        Section newSection = new() { FormVersionId = Guid.NewGuid() };
        FormVersion newFormVersion = new() { Id = newSection.FormVersionId };
        var dbSet = new List<Section>();
        var formVersions = new List<FormVersion>() { newFormVersion };

        _context.SetupGet(c => c.Sections).ReturnsDbSet(dbSet);
        _context.SetupGet(c => c.FormVersions).ReturnsDbSet(formVersions);

        // Act
        var result = await _sut.Create(newSection);

        // Assert
        _context.Verify(c => c.Sections.Add(newSection), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());

        Assert.True(result.Id != Guid.Empty);
    }
}
