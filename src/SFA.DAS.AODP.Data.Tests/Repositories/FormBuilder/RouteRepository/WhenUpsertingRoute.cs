using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.RouteRepository;

public class When_Upsert_Route
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.RouteRepository _sut;

    public When_Upsert_Route() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Upsert_Route()
    {
        // Arrange
        Guid routeId = Guid.NewGuid();

        Route newRoute = new()
        {
            //Id = routeId,
        };

        var dbSet = new List<Route>() { newRoute };

        _context.SetupGet(c => c.Routes).ReturnsDbSet(dbSet);

        // Act
        await _sut.UpsertAsync(dbSet);

        // Assert
        _context.Verify(c => c.Routes.Add(newRoute), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
    }
}
