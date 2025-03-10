using AutoFixture;
using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationReviewFeedback;

public class WhenGettingApplicationReviewFunding
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IApplicationDbContext> _context = new();
    private readonly Data.Repositories.Application.ApplicationReviewFundingRepository _sut;
    public WhenGettingApplicationReviewFunding() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_Applications_With_Count()
    {
        // Arrange
        var feedback = new Entities.Application.ApplicationReviewFunding()
        {
            ApplicationReviewId = Guid.NewGuid(),
        };

        _context.SetupGet(c => c.ApplicationReviewFundings).ReturnsDbSet([feedback]);


        // Act
        var result = await _sut.GetByReviewIdAsync(feedback.ApplicationReviewId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(feedback, result.First());
    }
}


