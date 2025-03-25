using Moq;
using SFA.DAS.AODP.Application.Commands.Application.Review;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Review;

public class SaveQfauFundingReviewOffersCommandHandlerTests
{
    private readonly Mock<IApplicationReviewFundingRepository> _applicationReviewFundingRepository = new();
    private readonly Mock<IApplicationReviewFeedbackRepository> _applicationReviewFeedbackRepository = new();
    private readonly SaveQfauFundingReviewOffersCommandHandler _saveQfauFundingReviewOffersCommandHandler;

    public SaveQfauFundingReviewOffersCommandHandlerTests()
    {
        _saveQfauFundingReviewOffersCommandHandler = new(_applicationReviewFundingRepository.Object, _applicationReviewFeedbackRepository.Object);
    }

    [Fact]
    public async Task Test_Funding_Offer_Old_Offers_Removed()
    {
        // Arrange
        var funding = new Data.Entities.Application.ApplicationReviewFunding()
        {
            FundingOfferId = Guid.NewGuid(),
            ApplicationReviewId = Guid.NewGuid(),
        };
        _applicationReviewFundingRepository.Setup(a => a.GetByReviewIdAsync(funding.ApplicationReviewId)).ReturnsAsync([funding]);


        // Act
        var response = await _saveQfauFundingReviewOffersCommandHandler.Handle(new()
        {
            ApplicationReviewId = funding.ApplicationReviewId,
            SelectedOfferIds = [Guid.NewGuid()]
        }, default);

        // Assert
        _applicationReviewFundingRepository.Verify(a => a.RemoveAsync(It.Is<List<ApplicationReviewFunding>>(a => a.Contains(funding))), Times.Once());
    }

    [Fact]
    public async Task Test_Funding_Offer_Added_If_Not_Exists()
    {
        // Arrange
        var funding = new Data.Entities.Application.ApplicationReviewFunding()
        {
            FundingOfferId = Guid.NewGuid(),
            ApplicationReviewId = Guid.NewGuid(),
        };
        _applicationReviewFundingRepository.Setup(a => a.GetByReviewIdAsync(funding.ApplicationReviewId)).ReturnsAsync([funding]);


        // Act
        var response = await _saveQfauFundingReviewOffersCommandHandler.Handle(new()
        {
            ApplicationReviewId = funding.ApplicationReviewId,
            SelectedOfferIds = [Guid.NewGuid()]
        }, default);

        // Assert
        _applicationReviewFundingRepository.Verify(a => a.CreateAsync(It.IsAny<List<ApplicationReviewFunding>>()), Times.Once());
    }

    [Fact]
    public async Task Test_Funding_Offer_Not_Added_If_Exists()
    {
        // Arrange
        var funding = new Data.Entities.Application.ApplicationReviewFunding()
        {
            FundingOfferId = Guid.NewGuid(),
            ApplicationReviewId = Guid.NewGuid(),
        };
        _applicationReviewFundingRepository.Setup(a => a.GetByReviewIdAsync(funding.ApplicationReviewId)).ReturnsAsync([funding]);


        // Act
        var response = await _saveQfauFundingReviewOffersCommandHandler.Handle(new()
        {
            ApplicationReviewId = funding.ApplicationReviewId,
            SelectedOfferIds = [funding.FundingOfferId]
        }, default);

        // Assert
        _applicationReviewFundingRepository.Verify(a => a.CreateAsync(It.IsAny<List<ApplicationReviewFunding>>()), Times.Never);

    }

    [Fact]
    public async Task Test_Exception_Thrown_For_Incomplete_ApplicationReviewId()
    {
        // Arrange
        var funding = new Data.Entities.Application.ApplicationReviewFunding()
        {
            ApplicationReviewId = Guid.NewGuid(),
        };

        _applicationReviewFundingRepository.Setup(a => a.GetByReviewIdAsync(funding.ApplicationReviewId)).ThrowsAsync(new Exception());

        // Act
        var response = await _saveQfauFundingReviewOffersCommandHandler.Handle(new()
        {
            ApplicationReviewId = funding.ApplicationReviewId,
        }, default);

        // Assert
        Assert.False(response.Success);
        Assert.IsAssignableFrom<Exception>(response.InnerException);
    }
}
