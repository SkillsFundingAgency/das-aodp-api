using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.RouteRepository;

public class WhenGettingQuestionRoutingDetailsByFormVersionId
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.RouteRepository _sut;

    public WhenGettingQuestionRoutingDetailsByFormVersionId() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_Question_Routing_Details_By_Form_Version_Id()
    {
        // Arrange
        Guid pageId = Guid.NewGuid();
        Guid routeId = Guid.NewGuid();

        Route newRoute = new()
        {
            Id = routeId,
            NextPageId = pageId
        };

        var dbSet = new List<Route>() { newRoute };

        _context.SetupGet(c => c.Routes).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.GetQuestionRoutingDetailsByFormVersionId(routeId);

        // Assert
        // Assert.NotNull(result);
        // Assert.Equal(dbSet, result);
    }
}
