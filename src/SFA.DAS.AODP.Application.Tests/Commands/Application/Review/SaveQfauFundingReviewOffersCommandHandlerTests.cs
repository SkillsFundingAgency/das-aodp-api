using Moq;
using SFA.DAS.AODP.Application.Commands.Application.Review;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Review;

public class SaveQfauFundingReviewOffersCommandHandlerTests
{
    private readonly Mock<IApplicationReviewFundingRepository> _applicationReviewFundingRepository = new();
    private readonly Mock<IApplicationReviewFeedbackRepository> _applicationReviewFeedbackRepository = new();
    private readonly Mock<IApplicationReviewRepository> _applicationReviewRepository = new();

    private readonly SaveQfauFundingReviewOffersCommandHandler _handler;

    public SaveQfauFundingReviewOffersCommandHandlerTests()
    {
        _handler = new SaveQfauFundingReviewOffersCommandHandler(
            _applicationReviewFundingRepository.Object,
            _applicationReviewFeedbackRepository.Object,
            _applicationReviewRepository.Object);
    }

    
    [Fact]
    public async Task Test_Funding_Offer_Old_Offers_Removed()
    {
        var reviewId = Guid.NewGuid();

        var funding = new ApplicationReviewFunding
        {
            FundingOfferId = Guid.NewGuid(),
            ApplicationReviewId = reviewId
        };

        _applicationReviewFundingRepository
            .Setup(a => a.GetByReviewIdAsync(reviewId))
            .ReturnsAsync([funding]);

        _applicationReviewRepository
            .Setup(x => x.GetOperationalStartDateForReview(reviewId))
            .ReturnsAsync((DateTime?)null);

        await _handler.Handle(new SaveQfauFundingReviewOffersCommand
        {
            ApplicationReviewId = reviewId,
            SelectedOfferIds = new List<Guid> { Guid.NewGuid() }
        }, TestContext.Current.CancellationToken);

        _applicationReviewFundingRepository.Verify(a =>
            a.RemoveAsync(It.Is<List<ApplicationReviewFunding>>(list => list.Contains(funding))),
            Times.Once);
    }

    [Fact]
    public async Task Test_Funding_Offer_Added_If_Not_Exists()
    {
        var reviewId = Guid.NewGuid();

        var existingFunding = new ApplicationReviewFunding
        {
            FundingOfferId = Guid.NewGuid(),
            ApplicationReviewId = reviewId
        };

        _applicationReviewFundingRepository
            .Setup(a => a.GetByReviewIdAsync(reviewId))
            .ReturnsAsync([existingFunding]);

        _applicationReviewRepository
            .Setup(x => x.GetOperationalStartDateForReview(reviewId))
            .ReturnsAsync((DateTime?)null);

        await _handler.Handle(new SaveQfauFundingReviewOffersCommand
        {
            ApplicationReviewId = reviewId,
            SelectedOfferIds = new List<Guid> { Guid.NewGuid() }
        }, TestContext.Current.CancellationToken);

        _applicationReviewFundingRepository.Verify(a =>
            a.CreateAsync(It.IsAny<List<ApplicationReviewFunding>>()),
            Times.Once);
    }

    [Fact]
    public async Task Test_Funding_Offer_Not_Added_If_Exists()
    {
        var reviewId = Guid.NewGuid();
        var offerId = Guid.NewGuid();

        var funding = new ApplicationReviewFunding
        {
            FundingOfferId = offerId,
            ApplicationReviewId = reviewId
        };

        _applicationReviewFundingRepository
            .Setup(a => a.GetByReviewIdAsync(reviewId))
            .ReturnsAsync([funding]);

        _applicationReviewRepository
            .Setup(x => x.GetOperationalStartDateForReview(reviewId))
            .ReturnsAsync((DateTime?)null);

        await _handler.Handle(new SaveQfauFundingReviewOffersCommand
        {
            ApplicationReviewId = reviewId,
            SelectedOfferIds = new List<Guid> { offerId }
        }, TestContext.Current.CancellationToken);

        _applicationReviewFundingRepository.Verify(a =>
            a.CreateAsync(It.IsAny<List<ApplicationReviewFunding>>()),
            Times.Never);
    }

    [Fact]
    public async Task Test_Exception_Thrown_For_Incomplete_ApplicationReviewId()
    {
        var reviewId = Guid.NewGuid();

        _applicationReviewFundingRepository
            .Setup(a => a.GetByReviewIdAsync(reviewId))
            .ThrowsAsync(new Exception());

        _applicationReviewRepository
            .Setup(x => x.GetOperationalStartDateForReview(reviewId))
            .ReturnsAsync((DateTime?)null);

        var response = await _handler.Handle(new SaveQfauFundingReviewOffersCommand
        {
            ApplicationReviewId = reviewId,
            SelectedOfferIds = new List<Guid>()
        }, default);

        Assert.False(response.Success);
        Assert.IsAssignableFrom<Exception>(response.InnerException);
    }

    [Fact]
    public async Task Test_Funding_Offer_Created_With_Null_StartDate_When_OperationalDate_Is_Null()
    {
        var reviewId = Guid.NewGuid();
        var newOfferId = Guid.NewGuid();

        _applicationReviewFundingRepository
            .Setup(x => x.GetByReviewIdAsync(reviewId))
            .ReturnsAsync(new List<ApplicationReviewFunding>());

        _applicationReviewRepository
            .Setup(x => x.GetOperationalStartDateForReview(reviewId))
            .ReturnsAsync((DateTime?)null);

        await _handler.Handle(new SaveQfauFundingReviewOffersCommand
        {
            ApplicationReviewId = reviewId,
            SelectedOfferIds = new List<Guid> { newOfferId }
        }, TestContext.Current.CancellationToken);

        _applicationReviewFundingRepository.Verify(x =>
            x.CreateAsync(It.Is<List<ApplicationReviewFunding>>(list =>
                list.Count == 1 &&
                list[0].FundingOfferId == newOfferId &&
                list[0].StartDate == null
            )),
            Times.Once);
    }

    [Fact]
    public async Task Test_Funding_Offer_Created_With_StartDate_When_OperationalDate_Exists()
    {
        var reviewId = Guid.NewGuid();
        var newOfferId = Guid.NewGuid();
        var operationalDate = new DateTime(2024, 01, 15);

        _applicationReviewFundingRepository
            .Setup(x => x.GetByReviewIdAsync(reviewId))
            .ReturnsAsync(new List<ApplicationReviewFunding>());

        _applicationReviewRepository
            .Setup(x => x.GetOperationalStartDateForReview(reviewId))
            .ReturnsAsync(operationalDate);

        await _handler.Handle(new SaveQfauFundingReviewOffersCommand
        {
            ApplicationReviewId = reviewId,
            SelectedOfferIds = new List<Guid> { newOfferId }
        }, TestContext.Current.CancellationToken);

        _applicationReviewFundingRepository.Verify(x =>
            x.CreateAsync(It.Is<List<ApplicationReviewFunding>>(list =>
                list.Count == 1 &&
                list[0].FundingOfferId == newOfferId &&
                list[0].StartDate == DateOnly.FromDateTime(operationalDate)
            )),
            Times.Once);
    }
}