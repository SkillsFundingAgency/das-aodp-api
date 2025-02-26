using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationPageRepository;

public class WhenUpsertingPages
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.Application.ApplicationPageRepository _sut;

    public WhenUpsertingPages() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Pages_Are_Upserted()
    {
        // Arrange
        ApplicationPage page = new();
        var dbSet = new List<ApplicationPage>();

        _context.SetupGet(c => c.ApplicationPages).ReturnsDbSet(dbSet);

        // Act
        await _sut.UpsertAsync(new List<ApplicationPage>() { page });

        // Assert
        _context.Verify(c => c.ApplicationPages.Add(page), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
    }
}




