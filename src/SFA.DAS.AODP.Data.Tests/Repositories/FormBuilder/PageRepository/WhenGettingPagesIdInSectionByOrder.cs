using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.PageRepository;

public class WhenGettingPagesIdInSectionByOrder
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.PageRepository _sut;

    public WhenGettingPagesIdInSectionByOrder() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_Pages_Id_In_Section_By_Order()
    {
        // Arrange
        Guid sectionId = Guid.NewGuid();
        Guid pageId1 = Guid.NewGuid();
        Guid pageId2 = Guid.NewGuid();
        Guid pageId3 = Guid.NewGuid();

        Page newPage1 = new()
        {
            Id = pageId1,
            Order = 0
        };

        Page newPage2 = new()
        {
            Id = pageId2,
            Order = 0
        };

        Page newPage3 = new()
        {
            Id = pageId2,
            Order = 0
        };

        Section newSection = new()
        {
            Id = sectionId,
            Pages = [
                newPage1,
                newPage2,
                newPage3
            ]
        };


        var expectedAnswer = new List<Guid>() { pageId1, pageId2 };

        var dbSet = new List<Section>() { newSection };

        _context.SetupGet(c => c.Sections).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.GetPagesIdInSectionByOrderAsync(sectionId, 0, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedAnswer, result);
    }
}
