using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.RouteRepository;

public class WhenDeleteRoute
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.RouteRepository _sut;

    public WhenDeleteRoute() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Delete_Route()
    {
        // Arrange
        Guid questionId = Guid.NewGuid();

        var dbSet = new List<Route>()
        {
            new()
            {
                SourceQuestionId = questionId,
            },
             new()
            {
                SourceQuestionId = Guid.NewGuid(),
            }
        };

        _context.SetupGet(c => c.Routes).ReturnsDbSet(dbSet);

        // Act
        await _sut.DeleteRouteByQuestionIdAsync(questionId);

        // Assert
        _context.Verify(c => c.Routes.RemoveRange(It.Is<List<Route>>(r => r.Count == 1)), Times.Once());
        _context.Verify(c => c.Routes.RemoveRange(It.Is<List<Route>>(r => r[0].SourceQuestionId == questionId)), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
    }

}
