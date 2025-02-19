using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.QuestionRepository;

public class WhenMovingSectionOrderDown
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.SectionRepository _sut;
    private readonly Mock<IPageRepository> _pageRepository = new();
    public WhenMovingSectionOrderDown() => _sut = new(
        _context.Object,
        _pageRepository.Object
    );

    [Fact]
    public async Task Then_Moving_Section_Order_Down()
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

        var dbSet = new List<Section>();

        _context.SetupGet(c => c.Sections).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.MoveSectionOrderDown(sectionId2);

        // Assert
        Assert.True(result);
        Assert.Equal(1, newSection1.Order);
        Assert.Equal(0, newSection2.Order);
    }
}