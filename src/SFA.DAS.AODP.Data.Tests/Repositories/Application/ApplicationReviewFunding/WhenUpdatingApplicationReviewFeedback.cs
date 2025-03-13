using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationRepository;

public class WhenUpdatingApplicationReviewFunding
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.Application.ApplicationReviewFundingRepository _sut;

    public WhenUpdatingApplicationReviewFunding() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_The_Funding_Is_Updated()
    {
        // Arrange
        List<Entities.Application.ApplicationReviewFunding> fundings = new();
        var dbSet = new List<Entities.Application.ApplicationReviewFunding>();

        _context.SetupGet(c => c.ApplicationReviewFundings).ReturnsDbSet(dbSet);

        // Act
         await _sut.UpdateAsync(fundings);

        // Assert
        _context.Verify(c => c.ApplicationReviewFundings.UpdateRange(fundings), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
    }
}
