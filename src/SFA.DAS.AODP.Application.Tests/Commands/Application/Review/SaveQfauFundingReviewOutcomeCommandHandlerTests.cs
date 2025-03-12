using Moq;
using SFA.DAS.AODP.Application.Commands.Application.Review;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Review;

public class SaveQfauFundingReviewOutcomeCommandHandlerTests
{
    private readonly Mock<IApplicationReviewFeedbackRepository> _repository = new();
    private readonly SaveQfauFundingReviewOutcomeCommandHandler _handler;

    public SaveQfauFundingReviewOutcomeCommandHandlerTests()
    {
        _handler = new(_repository.Object);
    }

    [Fact]
    public async Task Test_Funding_Details_Updated()
    {
        // Arrange
        var funding = new Data.Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReviewId = Guid.NewGuid(),

        };

        _repository.Setup(a => a.GetApplicationReviewFeedbackDetailsByReviewIdAsync(funding.ApplicationReviewId, Models.Application.UserType.Qfau)).ReturnsAsync(funding);

        // Act
        var response = await _handler.Handle(new()
        {
            ApplicationReviewId = funding.ApplicationReviewId,
            Approved = true
        }, default);

        // Assert
        _repository.Verify(a => a.UpdateAsync(funding), Times.Once());
        Assert.Equal("Approved", funding.Status);
    }

    [Fact]
    public async Task Test_Funding_Details_Updated_And_Correct_Status()
    {
        // Arrange
        var funding = new Data.Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReviewId = Guid.NewGuid(),

        };

        _repository.Setup(a => a.GetApplicationReviewFeedbackDetailsByReviewIdAsync(funding.ApplicationReviewId, Models.Application.UserType.Qfau)).ReturnsAsync(funding);

        // Act
        var response = await _handler.Handle(new()
        {
            ApplicationReviewId = funding.ApplicationReviewId,
            Approved = false
        }, default);

        // Assert
        _repository.Verify(a => a.UpdateAsync(funding), Times.Once());
        Assert.Equal("NotApproved", funding.Status);
    }

    [Fact]
    public async Task Test_Exception_Thrown_For_Invalid_Offer_Id()
    {
        // Arrange
        var funding = new Data.Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReviewId = Guid.NewGuid(),
        };

        _repository.Setup(a => a.GetApplicationReviewFeedbackDetailsByReviewIdAsync(funding.ApplicationReviewId, Models.Application.UserType.Qfau)).ThrowsAsync(new Exception());

        // Act
        var response = await _handler.Handle(new()
        {
            ApplicationReviewId = funding.ApplicationReviewId,
        }, default);

        // Assert
        Assert.False(response.Success);
        Assert.IsAssignableFrom<Exception>(response.InnerException);
    }

}
