using Moq;
using SFA.DAS.AODP.Application.Commands.Application.Review;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Review;

public class SaveQfauFundingReviewOffersDetailsCommandHandlerTests
{
    private readonly Mock<IApplicationReviewFundingRepository> _applicationReviewFundingRepository = new();
    private readonly SaveQfauFundingReviewOffersDetailsCommandHandler _handler;

    public SaveQfauFundingReviewOffersDetailsCommandHandlerTests()
    {
        _handler = new(_applicationReviewFundingRepository.Object);
    }

    [Fact]
    public async Task Test_Funding_Details_Updated()
    {
        // Arrange
        var funding = new Data.Entities.Application.ApplicationReviewFunding()
        {
            ApplicationReviewId = Guid.NewGuid(),
            FundingOfferId = Guid.NewGuid(),

        };

        _applicationReviewFundingRepository.Setup(a => a.GetByReviewIdAsync(funding.ApplicationReviewId)).ReturnsAsync([funding]);

        // Act
        SaveQfauFundingReviewOffersDetailsCommand request = new()
        {
            ApplicationReviewId = funding.ApplicationReviewId,
            Details = new()
            {
                new()
                {
                    FundingOfferId = funding.FundingOfferId,
                    StartDate = DateOnly.MinValue,
                    EndDate = DateOnly.MaxValue,
                    Comments ="test"
                }
            }
        };
        var response = await _handler.Handle(request, default);

        // Assert
        _applicationReviewFundingRepository.Verify(a => a.UpdateAsync(It.IsAny<List<ApplicationReviewFunding>>()), Times.Once());

        Assert.Equal(request.Details.First().StartDate, funding.StartDate);
        Assert.Equal(request.Details.First().EndDate, funding.EndDate);
        Assert.Equal(request.Details.First().Comments, funding.Comments);
    }

    [Fact]
    public async Task Test_Exception_Thrown_For_Invalid_Offer_Id()
    {
        // Arrange
        var funding = new Data.Entities.Application.ApplicationReviewFunding()
        {
            ApplicationReviewId = Guid.NewGuid(),
        };

        _applicationReviewFundingRepository.Setup(a => a.GetByReviewIdAsync(funding.ApplicationReviewId)).ReturnsAsync([funding]);

        // Act
        var response = await _handler.Handle(new()
        {
            ApplicationReviewId = funding.ApplicationReviewId,
            Details = new()
            {
                new()
                {
                     FundingOfferId = Guid.NewGuid(),
                }
            }
        }, default);

        // Assert
        Assert.False(response.Success);
        Assert.IsAssignableFrom<RecordNotFoundException>(response.InnerException);
    }

}
