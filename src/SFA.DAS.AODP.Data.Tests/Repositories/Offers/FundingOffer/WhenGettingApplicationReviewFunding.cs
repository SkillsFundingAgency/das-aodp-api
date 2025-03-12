using AutoFixture;
using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationReviewFeedback;

public class WhenGettingOffers
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IApplicationDbContext> _context = new();
    private readonly Data.Repositories.FundingOffer.FundingOfferRepository _sut;
    public WhenGettingOffers() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_Applications_With_Count()
    {
        // Arrange
        var offer = new Entities.Offer.FundingOffer();

        _context.SetupGet(c => c.FundingOffers).ReturnsDbSet([offer]);


        // Act
        var result = await _sut.GetFundingOffersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(offer, result.First());
    }
}


