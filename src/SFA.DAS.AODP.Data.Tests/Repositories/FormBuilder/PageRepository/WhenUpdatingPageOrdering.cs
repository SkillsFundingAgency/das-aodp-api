using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.PageRepository;

public class WhenUpdatingPageOrdering
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.PageRepository _sut;

    public WhenUpdatingPageOrdering() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Update_Page_Ordering()
    {
        // // Arrange
        // Page newPage = new();
        // var dbSet = new List<Page>();

        // _context.SetupGet(c => c.Pages).ReturnsDbSet(dbSet);

        // // Act
        // var result = await _sut.UpdatePageOrdering(newPage);

        // // Assert
        // _context.Verify(c => c.Pages.Update(newPage), Times.Once());
        // _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
    }
}
