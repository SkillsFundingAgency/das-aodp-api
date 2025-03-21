using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationRepository;

public class WhenCheckingApplicationReviewExistsForApplication
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.Application.ApplicationReviewRepository _sut;

    public WhenCheckingApplicationReviewExistsForApplication() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_The_Result_Is_True_When_Review_Exists()
    {
        // Arrange
        Entities.Application.ApplicationReview review = new()
        {
            ApplicationId = Guid.NewGuid(),
        };

        _context.SetupGet(c => c.ApplicationReviews).ReturnsDbSet([review]);

        // Act
        var result = await _sut.CheckIfReviewExistsByApplicationIdAsync(review.ApplicationId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Then_The_Result_Is_False_When_Review_Does_Not_Exist()
    {
        // Arrange
        Entities.Application.ApplicationReview review = new()
        {
            ApplicationId = Guid.NewGuid(),
        };

        _context.SetupGet(c => c.ApplicationReviews).ReturnsDbSet([review]);

        // Act
        var result = await _sut.CheckIfReviewExistsByApplicationIdAsync(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }
}
