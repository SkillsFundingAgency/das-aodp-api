using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationRepository;

public class WhenCreatingApplication
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.Application.ApplicationRepository _sut;

    public WhenCreatingApplication() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_The_Application_Is_Created()
    {
        // Arrange
        Entities.Application.Application application = new();
        var dbSet = new List<Entities.Application.Application>();

        _context.SetupGet(c => c.Applications).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.Create(application);

        // Assert
        _context.Verify(c => c.Applications.AddAsync(application, default), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());

        Assert.True(result.Id != Guid.Empty);
    }
}


