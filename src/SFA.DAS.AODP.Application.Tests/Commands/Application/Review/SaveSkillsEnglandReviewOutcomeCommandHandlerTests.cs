using MediatR;
using Moq;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Application.Commands.Application.Review;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Review;

public class SaveSkillsEnglandReviewOutcomeCommandHandlerTests
{
    private readonly Mock<IApplicationReviewFeedbackRepository> _applicationReviewFeedbackRepository = new();
    private readonly Mock<IMediator> _mediator = new();
    private readonly SaveSkillsEnglandReviewOutcomeCommandHandler _handler;

    private const string SkillsEnglandUserEmail = "skillsengland@test.com";
    private const string SkillsEnglandUserName = "Skills England User";
    private const string OutcomeComments = "Looks good";

    private const ApplicationStatus ApprovedStatus = ApplicationStatus.Approved;
    private const ApplicationStatus NotApprovedStatus = ApplicationStatus.NotApproved;

    private const string NotificationTemplateName = EmailTemplateNames.QFASTSubmittedApplicationChangedNotification;
    private const NotificationRecipientKind NotificationRecipientKindConst = NotificationRecipientKind.QfauMailbox;

    public SaveSkillsEnglandReviewOutcomeCommandHandlerTests()
    {
        _handler = new SaveSkillsEnglandReviewOutcomeCommandHandler(
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
    public async Task Test_Outcome_Not_Shared_With_SkillsEngland_Returns_Error()
    {
        // Arrange
        var reviewId = Guid.NewGuid();

        var review = new ApplicationReviewFeedback
        {
            ApplicationReviewId = reviewId,
            ApplicationReview = new ApplicationReview
            {
                SharedWithSkillsEngland = false
            }
        };

        _applicationReviewFeedbackRepository
            .Setup(r => r.GeyByReviewIdAndUserType(reviewId, UserType.SkillsEngland))
            .ReturnsAsync(review);

        var command = new SaveSkillsEnglandReviewOutcomeCommand
        {
            ApplicationReviewId = reviewId,
            Approved = true,
            Comments = OutcomeComments,
            SentByEmail = SkillsEnglandUserEmail,
            SentByName = SkillsEnglandUserName
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
    public async Task Test_Approved_Outcome_Updated_And_Notifications_Returned()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();

        var review = new ApplicationReviewFeedback
        {
            ApplicationReviewId = reviewId,
            Comments = null,
            Status = ApplicationStatus.InReview.ToString(),
            ApplicationReview = new ApplicationReview
            {
                Id = Guid.NewGuid(),
                ApplicationId = applicationId,
                SharedWithSkillsEngland = true
            }
        };

        _applicationReviewFeedbackRepository
            .Setup(r => r.GeyByReviewIdAndUserType(reviewId, UserType.SkillsEngland))
            .ReturnsAsync(review);

        var command = new SaveSkillsEnglandReviewOutcomeCommand
        {
            ApplicationReviewId = reviewId,
            Approved = true,
            Comments = OutcomeComments,
            SentByEmail = SkillsEnglandUserEmail,
            SentByName = SkillsEnglandUserName
        };

        var expectedStatusString = ApprovedStatus.ToString();
        var expectedMessageText =
            $"Status: {expectedStatusString} \n Comments: \n {OutcomeComments}";

        // Act
        var response = await _handler.Handle(command, default);

        // Assert
        Assert.Multiple(() =>
        {
            _applicationReviewFeedbackRepository.Verify(r =>
                    r.UpdateAsync(It.Is<ApplicationReviewFeedback>(x =>
                        x.Comments == OutcomeComments &&
                        x.Status == expectedStatusString)),
                Times.Once);

            _mediator.Verify(m => m.Send(
                    It.Is<CreateApplicationMessageCommand>(c =>
                        c.ApplicationId == applicationId &&
                        c.MessageType == MessageType.SkillsEnglandFeedbackSubmitted.ToString() &&
                        c.UserType == UserType.SkillsEngland.ToString() &&
                        c.SentByEmail == SkillsEnglandUserEmail &&
                        c.SentByName == SkillsEnglandUserName &&
                        c.MessageText == expectedMessageText),
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
    public async Task Test_NotApproved_Outcome_Updated_With_NotApproved_Status()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();

        var review = new ApplicationReviewFeedback
        {
            ApplicationReviewId = reviewId,
            Comments = null,
            Status = ApplicationStatus.InReview.ToString(),
            ApplicationReview = new ApplicationReview
            {
                Id = Guid.NewGuid(),
                ApplicationId = applicationId,
                SharedWithSkillsEngland = true
            }
        };

        _applicationReviewFeedbackRepository
            .Setup(r => r.GeyByReviewIdAndUserType(reviewId, UserType.SkillsEngland))
            .ReturnsAsync(review);

        var command = new SaveSkillsEnglandReviewOutcomeCommand
        {
            ApplicationReviewId = reviewId,
            Approved = false,
            Comments = OutcomeComments,
            SentByEmail = SkillsEnglandUserEmail,
            SentByName = SkillsEnglandUserName
        };

        var expectedStatusString = NotApprovedStatus.ToString();
        var expectedMessageText =
            $"Status: {expectedStatusString} \n Comments: \n {OutcomeComments}";

        // Act
        var response = await _handler.Handle(command, default);

        // Assert
        Assert.Multiple(() =>
        {
            _applicationReviewFeedbackRepository.Verify(r =>
                    r.UpdateAsync(It.Is<ApplicationReviewFeedback>(x =>
                        x.Comments == OutcomeComments &&
                        x.Status == expectedStatusString)),
                Times.Once);

            _mediator.Verify(m => m.Send(
                    It.Is<CreateApplicationMessageCommand>(c =>
                        c.ApplicationId == applicationId &&
                        c.MessageType == MessageType.SkillsEnglandFeedbackSubmitted.ToString() &&
                        c.UserType == UserType.SkillsEngland.ToString() &&
                        c.SentByEmail == SkillsEnglandUserEmail &&
                        c.SentByName == SkillsEnglandUserName &&
                        c.MessageText == expectedMessageText),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            Assert.True(response.Success);
            Assert.NotNull(response.Value);
            Assert.NotNull(response.Value.Notifications);
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
                SharedWithSkillsEngland = true
            }
        };

        _applicationReviewFeedbackRepository
            .Setup(r => r.GeyByReviewIdAndUserType(reviewId, UserType.SkillsEngland))
            .ReturnsAsync(review);

        _mediator
            .Setup(m => m.Send(
                It.IsAny<CreateApplicationMessageCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse>
            {
                Success = false,
                ErrorMessage = "Boom"
            });

        var command = new SaveSkillsEnglandReviewOutcomeCommand
        {
            ApplicationReviewId = reviewId,
            Approved = true,
            Comments = OutcomeComments,
            SentByEmail = SkillsEnglandUserEmail,
            SentByName = SkillsEnglandUserName
        };

        // Act
        var response = await _handler.Handle(command, default);

        // Assert
        Assert.False(response.Success);
        Assert.NotNull(response.InnerException);

        var expectedStatusString = ApprovedStatus.ToString();

        _applicationReviewFeedbackRepository.Verify(r =>
                r.UpdateAsync(It.Is<ApplicationReviewFeedback>(x =>
                    x.Comments == OutcomeComments &&
                    x.Status == expectedStatusString)),
            Times.Once);

        _mediator.Verify(m =>
                m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
