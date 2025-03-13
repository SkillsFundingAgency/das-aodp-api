using AutoFixture;
using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationReviewFeedback;

public class WhenGettingApplicationReview
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IApplicationDbContext> _context = new();
    private readonly Data.Repositories.Application.ApplicationReviewRepository _sut;
    public WhenGettingApplicationReview() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_Review_By_Id()
    {
        // Arrange
        var review = new Entities.Application.ApplicationReview()
        {
            Id = Guid.NewGuid(),
        };

        _context.SetupGet(c => c.ApplicationReviews).ReturnsDbSet([review]);


        // Act
        var result = await _sut.GetByIdAsync(review.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(review, result);
    }

    [Fact]
    public async Task Then_Get_Review_By_Application_Id()
    {
        // Arrange
        var review = new Entities.Application.ApplicationReview()
        {
            ApplicationId = Guid.NewGuid(),
        };

        _context.SetupGet(c => c.ApplicationReviews).ReturnsDbSet([review]);


        // Act
        var result = await _sut.GetByApplicationIdAsync(review.ApplicationId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(review, result);
    }


    [Fact]
    public async Task Then_Get_Review_With_Application_Details()
    {
        // Arrange
        var review = new Entities.Application.ApplicationReview()
        {
            Id = Guid.NewGuid(),
        };

        _context.SetupGet(c => c.ApplicationReviews).ReturnsDbSet([review]);


        // Act
        var result = await _sut.GetApplicationForReviewByReviewIdAsync(review.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(review, result);
    }
}


