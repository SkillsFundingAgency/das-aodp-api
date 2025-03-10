using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationRepository;

public class WhenCreatingApplicationReview
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.Application.ApplicationReviewRepository _sut;

    public WhenCreatingApplicationReview() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_The_Review_Is_Created()
    {
        // Arrange
        var review = new ApplicationReview();
        var dbSet = new List<Entities.Application.ApplicationReview>();

        _context.SetupGet(c => c.ApplicationReviews).ReturnsDbSet(dbSet);

        // Act
         await _sut.CreateAsync(review);

        // Assert
        _context.Verify(c => c.ApplicationReviews.Add(review), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());

        Assert.True(review.Id != Guid.Empty);

    }
}
