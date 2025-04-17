using MediatR;
using Moq;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Application.Commands.Application.Review;
using SFA.DAS.AODP.Application.Commands.Qualifications;
using SFA.DAS.AODP.Data.Entities.Offer;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.FundingOffer;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Models.Application;
using ProcessStatus = SFA.DAS.AODP.Data.Enum.ProcessStatus;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Review;

public class SaveQfauFundingReviewDecisionCommandHandlerTests
{
    private readonly Mock<IApplicationReviewFeedbackRepository> _reviewRepository = new();
    private readonly Mock<IMediator> _mediator = new();
    private readonly Mock<IQualificationDetailsRepository> _qualificationDetailsRepository = new();
    private readonly Mock<IQualificationFundingsRepository> _qualificationFundingsrepository = new();
    private readonly Mock<IQualificationDiscussionHistoryRepository> _qualificationDiscussionHistoryRepository = new();
    private readonly Mock<IQualificationsRepository> _qualificationsRepository = new();

    private readonly SaveQfauFundingReviewDecisionCommandHandler _handler;

    public SaveQfauFundingReviewDecisionCommandHandlerTests()
    {
        _handler = new(_reviewRepository.Object, _mediator.Object, _qualificationDetailsRepository.Object, _qualificationDiscussionHistoryRepository.Object,
            _qualificationFundingsrepository.Object, _qualificationsRepository.Object);
    }

    [Fact]
    public async Task Test_Funding_Details_Updated_For_Rejected_Applications_Without_Qan()
    {
        // Arrange
        var funding = new Data.Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReviewId = Guid.NewGuid(),
            Status = ApplicationStatus.NotApproved.ToString(),
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                }
            }
        };

        _mediator.Setup(mediator => mediator.Send(It.IsAny<CreateApplicationMessageCommand>(), default)).ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse>() { Success = true });
        _reviewRepository.Setup(a => a.GeyByReviewIdAndUserType(funding.ApplicationReviewId, Models.Application.UserType.Qfau)).ReturnsAsync(funding);

        // Act
        var response = await _handler.Handle(new()
        {
            ApplicationReviewId = funding.ApplicationReviewId,
        }, default);

        // Assert
        Assert.True(response.Success);
        _reviewRepository.Verify(a => a.UpdateAsync(funding), Times.Once());
        _mediator.Verify(mediator => mediator.Send(It.IsAny<CreateApplicationMessageCommand>(), default), Times.Once());
    }

    [Fact]
    public async Task Test_Funding_Details_Updated_For_Approved_Applications_With_Qan()
    {
        // Arrange
        var qan = "123";
        var funding = new Data.Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReviewId = Guid.NewGuid(),
            Status = ApplicationStatus.Approved.ToString(),
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    QualificationNumber = qan
                }
            }
        };

        var qualVersion = new QualificationVersions()
        {
            Id = Guid.NewGuid(),
            Version = 1,
            ProcessStatus = new()
            {
                Name = ProcessStatus.DecisionRequired
            },
            Qualification = new()
            {
                Qan = qan,
            }
        };

        var offer = new FundingOffer()
        {
            Id = Guid.NewGuid()
        };

        var qualificationFundings = new List<QualificationFundings>()
        {
            new()
            {
                FundingOfferId = offer.Id,
            }
        };

        _qualificationFundingsrepository.Setup(q => q.GetByIdAsync(qualVersion.Id)).ReturnsAsync(qualificationFundings);
        _mediator.Setup(mediator => mediator.Send(It.IsAny<CreateApplicationMessageCommand>(), default)).ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse>() { Success = true });
        _reviewRepository.Setup(a => a.GeyByReviewIdAndUserType(funding.ApplicationReviewId, Models.Application.UserType.Qfau)).ReturnsAsync(funding);

        _qualificationDetailsRepository.Setup(q => q.GetQualificationDetailsByIdAsync(qan)).ReturnsAsync(qualVersion);

        _mediator.Setup(mediator => mediator.Send(It.IsAny<SaveQualificationsFundingOffersOutcomeCommand>(), default)).ReturnsAsync(new BaseMediatrResponse<EmptyResponse>() { Success = true });
        _mediator.Setup(mediator => mediator.Send(It.IsAny<SaveQualificationsFundingOffersCommand>(), default)).ReturnsAsync(new BaseMediatrResponse<EmptyResponse>() { Success = true });
        _mediator.Setup(mediator => mediator.Send(It.IsAny<SaveQualificationsFundingOffersDetailsCommand>(), default)).ReturnsAsync(new BaseMediatrResponse<EmptyResponse>() { Success = true });

        // Act
        var response = await _handler.Handle(new()
        {
            ApplicationReviewId = funding.ApplicationReviewId,
        }, default);

        // Assert
        Assert.True(response.Success);
        _reviewRepository.Verify(a => a.UpdateAsync(funding), Times.Once());

        _mediator.Verify(mediator => mediator.Send(It.IsAny<CreateApplicationMessageCommand>(), default), Times.Once());
        _mediator.Verify(mediator => mediator.Send(It.IsAny<SaveQualificationsFundingOffersOutcomeCommand>(), default), Times.Once());
        _mediator.Verify(mediator => mediator.Send(It.IsAny<SaveQualificationsFundingOffersCommand>(), default), Times.Once());
        _mediator.Verify(mediator => mediator.Send(It.IsAny<SaveQualificationsFundingOffersDetailsCommand>(), default), Times.Once());

        _qualificationsRepository.Verify(q => q.UpdateQualificationStatus(qan, It.Is<Guid>(g => g == Guid.Parse("00000000-0000-0000-0000-000000000004")), qualVersion.Version.Value));
        _qualificationDiscussionHistoryRepository.Verify(q => q.CreateAsync(It.IsAny<QualificationDiscussionHistory>()), Times.Once);
    }

    [Fact]
    public async Task Test_Exception_Thrown_For_Missing_Qan_For_Approved_Application()
    {
        // Arrange
        var funding = new Data.Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReviewId = Guid.NewGuid(),
            Status = ApplicationStatus.Approved.ToString()
        };

        _reviewRepository.Setup(a => a.GeyByReviewIdAndUserType(funding.ApplicationReviewId, Models.Application.UserType.Qfau)).ReturnsAsync(funding);

        // Act
        var response = await _handler.Handle(new()
        {
            ApplicationReviewId = funding.ApplicationReviewId,
        }, default);

        // Assert
        Assert.False(response.Success);
        Assert.IsAssignableFrom<InvalidOperationException>(response.InnerException);
        Assert.Contains("No QAN has been provided for the application.", response.ErrorMessage);
    }

    [Fact]
    public async Task Test_Exception_Thrown_For_Invalid_Qan_For_Approved_Application()
    {
        // Arrange
        var funding = new Data.Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReviewId = Guid.NewGuid(),
            Status = ApplicationStatus.Approved.ToString(),
            ApplicationReview = new()
            {
                Application = new()
                {
                    QualificationNumber = "123"
                }
            }
        };

        _reviewRepository.Setup(a => a.GeyByReviewIdAndUserType(funding.ApplicationReviewId, Models.Application.UserType.Qfau)).ReturnsAsync(funding);

        // Act
        var response = await _handler.Handle(new()
        {
            ApplicationReviewId = funding.ApplicationReviewId,
        }, default);

        // Assert
        Assert.False(response.Success);
        Assert.IsAssignableFrom<InvalidOperationException>(response.InnerException);
        Assert.Contains("The current qualification status is not valid to confirm the decision.", response.ErrorMessage);
    }

    [Fact]
    public async Task Test_Exception_Thrown_For_Rejected_Application_With_Approved_Offers()
    {
        // Arrange
        var funding = new Data.Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReviewId = Guid.NewGuid(),
            Status = ApplicationStatus.NotApproved.ToString(),
            ApplicationReview = new()
            {
                ApplicationReviewFundings = new()
                {
                    new()
                }
            }
        };

        _reviewRepository.Setup(a => a.GeyByReviewIdAndUserType(funding.ApplicationReviewId, Models.Application.UserType.Qfau)).ReturnsAsync(funding);

        // Act
        var response = await _handler.Handle(new()
        {
            ApplicationReviewId = funding.ApplicationReviewId,
        }, default);

        // Assert
        Assert.False(response.Success);
        Assert.IsAssignableFrom<InvalidOperationException>(response.InnerException);
        Assert.Contains("The application has been rejected for funding but has approved offers associated.", response.ErrorMessage);
    }
}
