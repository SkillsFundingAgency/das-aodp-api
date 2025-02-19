using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.SectionRepository;

public class WhenDeletingSection
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.SectionRepository _sut;

    private readonly Mock<IPageRepository> _pageRepository = new();

    public WhenDeletingSection() => _sut = new(
        _context.Object,
        _pageRepository.Object
    );

    [Fact]
    public async Task Then_Delete_Section()
    {
        // Arrange
        Guid sectionId = Guid.NewGuid();

        Section newSection = new()
        {
            Id = sectionId
        };
        var dbSet = new List<Section>{ newSection };

        _context.SetupGet(c => c.Sections).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.DeleteSection(sectionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newSection, result);
    }
}
