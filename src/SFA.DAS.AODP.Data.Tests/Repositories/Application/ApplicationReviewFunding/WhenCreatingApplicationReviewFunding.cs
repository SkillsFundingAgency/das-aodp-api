using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationRepository;

public class WhenCreatingApplicationReviewFunding
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.Application.ApplicationReviewFundingRepository _sut;

    public WhenCreatingApplicationReviewFunding() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_The_Funding_Is_Created()
    {
        // Arrange
        var funding = new ApplicationReviewFunding();
        List<Entities.Application.ApplicationReviewFunding> fundings = [funding];
        var dbSet = new List<Entities.Application.ApplicationReviewFunding>();

        _context.SetupGet(c => c.ApplicationReviewFundings).ReturnsDbSet(dbSet);

        // Act
         await _sut.CreateAsync(fundings);

        // Assert
        _context.Verify(c => c.ApplicationReviewFundings.Add(funding), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());

        Assert.True(funding.Id != Guid.Empty);

    }
}
