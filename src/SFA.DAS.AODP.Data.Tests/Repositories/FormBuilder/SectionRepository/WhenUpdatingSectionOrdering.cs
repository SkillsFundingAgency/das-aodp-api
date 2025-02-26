using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.SectionRepository;

public class WhenUpdatingSectionOrdering
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.SectionRepository _sut;
    private readonly Mock<IPageRepository> _pageRepository = new();

    public WhenUpdatingSectionOrdering() => _sut = new(
        _context.Object,
        _pageRepository.Object
    );

    [Fact]
    public async Task Then_Update_Section_Ordering()
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
