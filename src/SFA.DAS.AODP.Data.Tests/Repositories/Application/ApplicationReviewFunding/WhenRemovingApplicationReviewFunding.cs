using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationRepository;

public class WhenRemovingApplicationReviewFunding
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.Application.ApplicationReviewFundingRepository _sut;

    public WhenRemovingApplicationReviewFunding() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_The_Funding_Is_Updated()
    {
        // Arrange
        List<Entities.Application.ApplicationReviewFunding> fundings = new();
        var dbSet = new List<Entities.Application.ApplicationReviewFunding>();

        _context.SetupGet(c => c.ApplicationReviewFundings).ReturnsDbSet(dbSet);

        // Act
        await _sut.RemoveAsync(fundings);

        // Assert
        _context.Verify(c => c.ApplicationReviewFundings.RemoveRange(fundings), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
    }
}