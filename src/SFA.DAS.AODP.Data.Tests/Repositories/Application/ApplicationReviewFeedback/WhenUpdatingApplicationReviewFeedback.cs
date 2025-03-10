using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationRepository;

public class WhenUpdatingApplicationReviewFeedback
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.Application.ApplicationReviewFeedbackRepository _sut;

    public WhenUpdatingApplicationReviewFeedback() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_The_Application_Is_Updated()
    {
        // Arrange
        Entities.Application.ApplicationReviewFeedback application = new();
        var dbSet = new List<Entities.Application.ApplicationReviewFeedback>();

        _context.SetupGet(c => c.ApplicationReviewFeedbacks).ReturnsDbSet(dbSet);

        // Act
         await _sut.UpdateAsync(application);

        // Assert
        _context.Verify(c => c.ApplicationReviewFeedbacks.Update(application), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
    }

    [Fact]
    public async Task Then_The_Application_Are_Updated()
    {
        // Arrange
        List<Entities.Application.ApplicationReviewFeedback> applications = new();
        var dbSet = new List<Entities.Application.ApplicationReviewFeedback>();

        _context.SetupGet(c => c.ApplicationReviewFeedbacks).ReturnsDbSet(dbSet);

        // Act
        await _sut.UpdateAsync(applications);

        // Assert
        _context.Verify(c => c.ApplicationReviewFeedbacks.UpdateRange(applications), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
    }
}
