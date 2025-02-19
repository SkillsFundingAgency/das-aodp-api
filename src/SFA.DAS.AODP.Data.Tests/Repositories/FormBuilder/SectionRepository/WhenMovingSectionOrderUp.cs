using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.QuestionRepository;

public class WhenMovingSectionOrderUp
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.SectionRepository _sut;
    private readonly Mock<IPageRepository> _pageRepository = new();
    public WhenMovingSectionOrderUp() => _sut = new(
        _context.Object,
        _pageRepository.Object
    );

    [Fact]
    public async Task Then_Moving_Section_Order_Up()
    {
        // Arrange
        Guid sectionId1 = Guid.NewGuid();

        Guid sectionId2 = Guid.NewGuid(); 

        Section newSection1 = new()
        {
            Id = sectionId1,
            Order = 0      
        };

        Section newSection2 = new()
        {
            Id = sectionId2,
            Order = 1
        };

        var dbSet = new List<Section>() { newSection1, newSection2 };

        _context.SetupGet(c => c.Sections).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.MoveSectionOrderDown(sectionId2);

        // Assert
        Assert.True(result);
    }
}