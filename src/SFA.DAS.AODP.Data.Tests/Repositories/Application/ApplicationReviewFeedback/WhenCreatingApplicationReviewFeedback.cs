using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationRepository;

public class WhenCreatingApplicationReviewFeedback
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.Application.ApplicationReviewFeedbackRepository _sut;

    public WhenCreatingApplicationReviewFeedback() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_The_Application_Is_Created()
    {
        // Arrange
        Entities.Application.ApplicationReviewFeedback application = new();
        var dbSet = new List<Entities.Application.ApplicationReviewFeedback>();

        _context.SetupGet(c => c.ApplicationReviewFeedbacks).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.CreateAsync(application);

        // Assert
        _context.Verify(c => c.ApplicationReviewFeedbacks.AddAsync(application, default), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());

        Assert.True(result.Id != Guid.Empty);
    }
}
