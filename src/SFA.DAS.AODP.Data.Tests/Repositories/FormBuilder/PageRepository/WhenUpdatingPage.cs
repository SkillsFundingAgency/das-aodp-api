using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.PageRepository;

public class WhenUpdatingPage
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.PageRepository _sut;

    public WhenUpdatingPage() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Update_Page()
    {
        // Arrange
        Page newPage = new() { Id = Guid.NewGuid() };
        var dbSet = new List<Page>() { newPage };

        _context.SetupGet(c => c.Pages).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.Update(newPage);

        // Assert
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
    }
}
