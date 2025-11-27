using MediatR;
using Moq;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Application.Commands.Application.Review;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Review;

public class SaveOfqualReviewOutcomeCommandHandlerTests
{
    private readonly Mock<IApplicationReviewFeedbackRepository> _applicationReviewFeedbackRepository = new();
    private readonly Mock<IMediator> _mediator = new();
    private readonly SaveOfqualReviewOutcomeCommandHandler _handler;

    private const string OfqualUserEmail = "ofqual@test.com";
    private const string OfqualUserName = "Ofqual User";
    private const string OutcomeComments = "Looks good";

    private const string NotificationTemplateName = EmailTemplateNames.QFASTSubmittedApplicationChangedNotification;
    private const NotificationRecipientKind NotificationRecipientKindConst = NotificationRecipientKind.QfauMailbox;

    private const ApplicationStatus ReviewedStatus = ApplicationStatus.Reviewed;
    private const ApplicationStatus InReviewStatus = ApplicationStatus.InReview; 

    public SaveOfqualReviewOutcomeCommandHandlerTests()
    {
        _handler = new SaveOfqualReviewOutcomeCommandHandler(
            _applicationReviewFeedbackRepository.Object,
            _mediator.Object);

        var notifications = new List<NotificationDefinition>
        {
            new()
            {
                TemplateName = NotificationTemplateName,
                RecipientKind = NotificationRecipientKindConst,
            }
        };

        _mediator
            .Setup(m => m.Send(
                It.IsAny<CreateApplicationMessageCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse>
            {
                Success = true,
                Value = new CreateApplicationMessageCommandResponse
                {
                    Notifications = notifications
                }
            });
    }

    [Fact]
    public async Task Test_Outcome_Not_Shared_With_Ofqual_Returns_Error()
    {
        // Arrange
        var reviewId = Guid.NewGuid();

        var review = new ApplicationReviewFeedback
        {
            ApplicationReviewId = reviewId,
            ApplicationReview = new ApplicationReview
            {
                SharedWithOfqual = false
            }
        };

        _applicationReviewFeedbackRepository
            .Setup(r => r.GeyByReviewIdAndUserType(reviewId, UserType.Ofqual))
            .ReturnsAsync(review);

        var command = new SaveOfqualReviewOutcomeCommand
        {
            ApplicationReviewId = reviewId,
            Comments = OutcomeComments,
            SentByEmail = OfqualUserEmail,
            SentByName = OfqualUserName
        };

        // Act
        var response = await _handler.Handle(command, default);

        // Assert
        Assert.False(response.Success);
        Assert.IsAssignableFrom<InvalidOperationException>(response.InnerException);

        _applicationReviewFeedbackRepository.Verify(r =>
                r.UpdateAsync(It.IsAny<ApplicationReviewFeedback>()),
            Times.Never);

        _mediator.Verify(m =>
                m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Test_Outcome_Updated_And_Notifications_Returned()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();

        var review = new ApplicationReviewFeedback
        {
            ApplicationReviewId = reviewId,
            Comments = null,
            Status = InReviewStatus.ToString(),
            ApplicationReview = new ApplicationReview
            {
                Id = Guid.NewGuid(),
                ApplicationId = applicationId,
                SharedWithOfqual = true
            }
        };

        _applicationReviewFeedbackRepository
            .Setup(r => r.GeyByReviewIdAndUserType(reviewId, UserType.Ofqual))
            .ReturnsAsync(review);

        var command = new SaveOfqualReviewOutcomeCommand
        {
            ApplicationReviewId = reviewId,
            Comments = OutcomeComments,
            SentByEmail = OfqualUserEmail,
            SentByName = OfqualUserName
        };

        // Act
        var response = await _handler.Handle(command, default);

        // Assert
        Assert.Multiple(() =>
        {
            _applicationReviewFeedbackRepository.Verify(r =>
                    r.UpdateAsync(It.Is<ApplicationReviewFeedback>(x =>
                        x.Comments == OutcomeComments &&
                        x.Status == ReviewedStatus.ToString())),
                Times.Once);

            _mediator.Verify(m => m.Send(
                    It.Is<CreateApplicationMessageCommand>(c =>
                        c.ApplicationId == applicationId &&
                        c.MessageType == MessageType.OfqualFeedbackSubmitted.ToString() &&
                        c.UserType == UserType.Ofqual.ToString() &&
                        c.SentByEmail == OfqualUserEmail &&
                        c.SentByName == OfqualUserName &&
                        c.MessageText == OutcomeComments),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            Assert.True(response.Success);
            Assert.NotNull(response.Value);
            Assert.NotNull(response.Value.Notifications);

            var notifications = response.Value.Notifications;
            Assert.Single(notifications);

            var notification = notifications[0];
            Assert.Equal(NotificationTemplateName, notification.TemplateName);
            Assert.Equal(NotificationRecipientKindConst, notification.RecipientKind);
        });
    }

    [Fact]
    public async Task Test_Exception_When_Message_Creation_Fails()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();

        var review = new ApplicationReviewFeedback
        {
            ApplicationReviewId = reviewId,
            ApplicationReview = new ApplicationReview
            {
                ApplicationId = applicationId,
                SharedWithOfqual = true
            }
        };

        _applicationReviewFeedbackRepository
            .Setup(r => r.GeyByReviewIdAndUserType(reviewId, UserType.Ofqual))
            .ReturnsAsync(review);

        _mediator
            .Setup(m => m.Send(
                It.IsAny<CreateApplicationMessageCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse>
            {
                Success = false,
                ErrorMessage = "Error"
            });

        var command = new SaveOfqualReviewOutcomeCommand
        {
            ApplicationReviewId = reviewId,
            Comments = OutcomeComments,
            SentByEmail = OfqualUserEmail,
            SentByName = OfqualUserName
        };

        // Act
        var response = await _handler.Handle(command, default);

        // Assert
        Assert.False(response.Success);
        Assert.NotNull(response.InnerException);

        _applicationReviewFeedbackRepository.Verify(r =>
                r.UpdateAsync(It.Is<ApplicationReviewFeedback>(x =>
                    x.Comments == OutcomeComments &&
                    x.Status == ReviewedStatus.ToString())),
            Times.Once);

        _mediator.Verify(m =>
                m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
