using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationRepository;

public class WhenUpdatingApplicationReview
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.Application.ApplicationReviewRepository _sut;

    public WhenUpdatingApplicationReview() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_The_Review_Is_Updated()
    {
        // Arrange
        Entities.Application.ApplicationReview review = new();

        _context.SetupGet(c => c.ApplicationReviews).ReturnsDbSet([review]);

        // Act
        await _sut.UpdateAsync(review);

        // Assert
        _context.Verify(c => c.ApplicationReviews.Update(review), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
    }
}
