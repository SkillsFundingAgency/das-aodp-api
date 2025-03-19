using Moq;
using SFA.DAS.AODP.Application.Commands.Application.Review;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Review;

public class UpdateApplicationReviewSharingHandlerTests
{
    private readonly Mock<IApplicationReviewRepository> _applicationReviewRepository = new();
    private readonly Mock<IApplicationReviewFeedbackRepository> _applicationReviewFeedbackRepository = new();

    private readonly UpdateApplicationReviewSharingHandler _handler;

    public UpdateApplicationReviewSharingHandlerTests()
    {
        _handler = new(_applicationReviewRepository.Object, _applicationReviewFeedbackRepository.Object);
    }

    [Fact]
    public async Task Test_Sharing_Details_Updated_For_Ofqual()
    {
        // Arrange
        var funding = new Data.Entities.Application.ApplicationReview()
        {
            Id = Guid.NewGuid(),
            ApplicationReviewFeedbacks = new()

        };

        _applicationReviewRepository.Setup(a => a.GetByIdAsync(funding.Id)).ReturnsAsync(funding);


        // Act
        var response = await _handler.Handle(new()
        {
            ApplicationReviewId = funding.Id,
            ShareApplication = true,
            ApplicationReviewUserType = Models.Application.UserType.Ofqual

        }, default);

        // Assert
        _applicationReviewFeedbackRepository.Verify(a => a.CreateAsync(It.Is<ApplicationReviewFeedback>(f => f.Type == "Ofqual")), Times.Once());
    }


    [Fact]
    public async Task Test_Sharing_Details_Updated_For_SkillsEngland()
    {
        // Arrange
        var funding = new Data.Entities.Application.ApplicationReview()
        {
            Id = Guid.NewGuid(),
            ApplicationReviewFeedbacks = new()

        };

        _applicationReviewRepository.Setup(a => a.GetByIdAsync(funding.Id)).ReturnsAsync(funding);


        // Act
        var response = await _handler.Handle(new()
        {
            ApplicationReviewId = funding.Id,
            ShareApplication = true,
            ApplicationReviewUserType = Models.Application.UserType.SkillsEngland

        }, default);

        // Assert
        _applicationReviewFeedbackRepository.Verify(a => a.CreateAsync(It.Is<ApplicationReviewFeedback>(f => f.Type == "SkillsEngland")), Times.Once());
    }

    [Fact]
    public async Task Test_Exception_Thrown_For_Invalid_Offer_Id()
    {
        // Arrange
        var funding = new Data.Entities.Application.ApplicationReview()
        {
            Id = Guid.NewGuid(),
            ApplicationReviewFeedbacks = new()

        };

        _applicationReviewRepository.Setup(a => a.GetByIdAsync(funding.Id)).ThrowsAsync(new Exception());


        // Act
        var response = await _handler.Handle(new()
        {
            ApplicationReviewId = funding.Id,
            ShareApplication = true,
            ApplicationReviewUserType = Models.Application.UserType.SkillsEngland

        }, default);

        // Assert
        Assert.False(response.Success);
        Assert.IsAssignableFrom<Exception>(response.InnerException);
    }

}
