using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.RouteRepository;

public class WhenGettingRoutesByQuestionId
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.RouteRepository _sut;

    public WhenGettingRoutesByQuestionId() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_Routes_By_Question_Id()
    {
        Guid routeId = Guid.NewGuid();
        Guid questionId = Guid.NewGuid();

        Route newRoute = new()
        {
            Id = routeId,
            SourceQuestionId = questionId
        };

        var dbSet = new List<Route>() { newRoute };

        _context.SetupGet(c => c.Routes).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.GetRoutesByQuestionId(questionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dbSet, result);
    }
}
