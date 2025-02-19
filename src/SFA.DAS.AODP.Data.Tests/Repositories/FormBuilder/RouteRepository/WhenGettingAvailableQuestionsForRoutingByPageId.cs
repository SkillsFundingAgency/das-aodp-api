using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.RouteRepository;

public class WhenGettingAvailableQuestionsForRoutingByPageId
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.RouteRepository _sut;

    public WhenGettingAvailableQuestionsForRoutingByPageId() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_Available_Questions_For_Routing_By_Page_Id()
    {
        // Arrange
        Guid pageId = Guid.NewGuid();
        Guid validRouteId = Guid.NewGuid();
        Guid validQuestionId = Guid.NewGuid();
        Guid invalidRouteId = Guid.NewGuid();
        Guid invalidQuestionId = Guid.NewGuid();

        Route validRoute = new()
        {
            Id = validRouteId,
            SourceQuestionId = validQuestionId,
            NextPageId = pageId
        };

        Route invalidRoute = new()
        {
            Id = invalidRouteId,
            SourceQuestionId = invalidQuestionId,
        };

        var solution = new List<View_AvailableQuestionsForRouting>() {};

        var dbSet = new List<Route>() { validRoute, invalidRoute };

        _context.SetupGet(c => c.Routes).ReturnsDbSet(dbSet);
        _context.SetupGet(c => c.View_AvailableQuestionsForRoutings).ReturnsDbSet(solution);

        // Act
        await _sut.GetAvailableQuestionsForRoutingByPageId(pageId);

        // Assert
        _context.Verify(c => c.Routes.Contains(validRoute), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
        // Assert.NotNull(result);
        // Assert.Equal(dbSet, result);
    }
}
