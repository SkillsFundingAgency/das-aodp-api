using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.RouteRepository;

public class WhenCopyingRoutesForNewFormVerson
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.RouteRepository _sut;

    public WhenCopyingRoutesForNewFormVerson() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Copy_Routes_For_New_FormVersion()
    {
        // Arrange
        Guid migrateFromPageId = Guid.NewGuid();
        Guid migrateToPageId = Guid.NewGuid();
        Guid routeId = Guid.NewGuid();

        Route newRoute = new()
        {
            Id = routeId,
            NextPageId = migrateFromPageId
        };

        var dbSet = new List<Route>() { newRoute };

        _context.SetupGet(c => c.Routes).ReturnsDbSet(dbSet);

        Dictionary<Guid,Guid> oldQuestionIds = new Dictionary<Guid, Guid>();

        Dictionary<Guid,Guid> oldNewPageIds = new Dictionary<Guid, Guid>
        {
            {migrateFromPageId, migrateToPageId}
        };

        Dictionary<Guid,Guid> oldNewSectionIds = new Dictionary<Guid, Guid>();

        Dictionary<Guid,Guid> oldNewOptionsIds = new Dictionary<Guid, Guid>();
        // Act
        await _sut.CopyRoutesForNewFormVersion(oldQuestionIds, oldNewPageIds, oldNewSectionIds, oldNewOptionsIds);

        // Assert
        // Assert.NotNull(result);
        // Assert.Contains(routeId, result.Keys);
    }
}
