using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.RouteRepository;

public class WhenIfTheRouteIsEditable
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.RouteRepository _sut;

    public WhenIfTheRouteIsEditable() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_If_Route_Is_Editable()
    {
        // Arrange
        Guid RouteId = Guid.NewGuid();

        Route newRoute = new()
        {
            Id = RouteId,
        };

        var dbSet = new List<Route>() { newRoute };

        _context.SetupGet(c => c.Routes).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.IsRouteEditable(RouteId);

        // Assert
        Assert.True(result);
    }
}
