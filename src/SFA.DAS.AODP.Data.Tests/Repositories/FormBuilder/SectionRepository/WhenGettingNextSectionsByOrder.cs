using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.SectionRepository;

public class WhenGettingNextSectionsByOrder
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.SectionRepository _sut;

    private readonly Mock<IPageRepository> _pageRepository = new();

    public WhenGettingNextSectionsByOrder() => _sut = new(
        _context.Object,
        _pageRepository.Object
    );
    [Fact]
    public async Task Then_Get_Next_Sections_By_Order()
    {
        // Arrange
        Guid formVersionId = Guid.NewGuid();
        Guid sectionId1 = Guid.NewGuid();
        Guid sectionId2 = Guid.NewGuid();

        Section newSection1 = new()
        {
            Id = sectionId1,
            FormVersionId = formVersionId,
            Order = 0
        };

        Section newSection2 = new()
        {
            Id = sectionId2,
            FormVersionId = formVersionId,
            Order = 1
        };

        var sections = new List<Section>() { newSection1, newSection2 };

        FormVersion newFormVersion = new()
        {
            Id = formVersionId,
            Sections = [
                newSection1,
                newSection2
            ]
        };

        var solution = new List<Section>() { newSection2 };

        var dbSet = new List<FormVersion>() { newFormVersion };

        _context.SetupGet(c => c.FormVersions).ReturnsDbSet(dbSet);
        _context.SetupGet(c => c.Sections).ReturnsDbSet(sections);

        // Act
        var result = await _sut.GetNextSectionsByOrderAsync(formVersionId, 0);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(solution, result);
    }
}
