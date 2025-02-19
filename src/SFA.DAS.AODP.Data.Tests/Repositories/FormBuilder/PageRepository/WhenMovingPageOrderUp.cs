using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.PageRepository;

public class WhenMovingFormOrderUp
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.PageRepository _sut;

    public WhenMovingFormOrderUp() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Moving_The_FormOrder_Up()
    {  
        // Arrange
        Guid pageId1 = Guid.NewGuid();

        Guid pageId2 = Guid.NewGuid(); 

        Page newPage1 = new()
        {
            Id = pageId1,
            Order = 0
        };

        Page newPage2 = new()
        {
            Id = pageId2,
            Order = 1
        };

        var dbSet = new List<Page>() { newPage1, newPage2 };

        _context.SetupGet(c => c.Pages).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.MovePageOrderUp(pageId1);

        // Assert
        Assert.True(result);
    }
}