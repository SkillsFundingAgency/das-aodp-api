using MediatR;
using Moq;
using SFA.DAS.AODP.Application.Commands.Application;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Application.Services;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Application;

public class SubmitApplicationCommandHandlerTests
{
    private readonly Mock<IApplicationRepository> _applicationRepository = new();
    private readonly Mock<IApplicationReviewRepository> _applicationReviewRepository = new();
    private readonly Mock<IApplicationReviewFeedbackRepository> _applicationReviewFeedbackRepository = new();
    private readonly SubmitApplicationCommandHandler _submitApplicationCommandHandler;
    private readonly Mock<IMediator> _mediator = new();

    public SubmitApplicationCommandHandlerTests()
    {
        _submitApplicationCommandHandler = new(_applicationRepository.Object,
            _applicationReviewRepository.Object, _applicationReviewFeedbackRepository.Object, _mediator.Object);

        var submittedNotifications = new List<NotificationDefinition>
        {
            new()
            {
                TemplateName = EmailTemplateNames.QFASTApplicationSubmittedNotification,
                RecipientKind = NotificationRecipientKind.QfauMailbox,
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
                    Notifications = submittedNotifications
                }
            });
    }

    [Fact]
    public async Task Test_Application_Submitted_And_Review_Feedback_Created()
    {
        // Arrange
        var application = new Data.Entities.Application.Application()
        {
            Id = Guid.NewGuid(),
        };

        var summary = new View_SectionSummaryForApplication()
        {
            RemainingPages = 0
        };

        _applicationRepository.Setup(a => a.GetByIdAsync(application.Id)).Returns(Task.FromResult(application));
        _applicationRepository.Setup(a => a.GetSectionSummaryByApplicationIdAsync(application.Id)).ReturnsAsync([summary]);


        // Act
        var response = await _submitApplicationCommandHandler.Handle(new()
        {
            ApplicationId = application.Id,
        }, default);

        // Assert
        _applicationReviewRepository.Verify(a => a.CreateAsync(It.Is<ApplicationReview>(a => a.ApplicationId == application.Id)));
        _applicationReviewFeedbackRepository.Verify(a => a.CreateAsync(It.Is<ApplicationReviewFeedback>(a => a.Type == UserType.Qfau.ToString())));
        _applicationRepository.Verify(a => a.UpdateAsync(application));
    }


    [Fact]
    public async Task Test_Application_Submitted_Review_Feedback_Updated_Notification_Definition_Returned()
    {
        // Arrange
        var application = new Data.Entities.Application.Application()
        {
            Id = Guid.NewGuid(),
        };

        var summary = new View_SectionSummaryForApplication()
        {
            RemainingPages = 0
        };

        var review = new ApplicationReview()
        {
            ApplicationReviewFeedbacks = new List<ApplicationReviewFeedback>()
            {
                new()
                {
                    NewMessage = false
                }
            }
        };

        _applicationRepository.Setup(a => a.GetByIdAsync(application.Id)).Returns(Task.FromResult(application));
        _applicationRepository.Setup(a => a.GetSectionSummaryByApplicationIdAsync(application.Id)).ReturnsAsync([summary]);
        _applicationReviewRepository.Setup(a => a.GetByApplicationIdAsync(application.Id)).ReturnsAsync(review);

        // Act
        var response = await _submitApplicationCommandHandler.Handle(new()
        {
            ApplicationId = application.Id,
        }, default);

        // Assert

        Assert.Multiple(() => 
        {
            _applicationReviewFeedbackRepository.Verify(a => a.UpdateAsync(It.Is<List<ApplicationReviewFeedback>>(a => a.All(r => r.NewMessage))));
            _applicationReviewFeedbackRepository.Verify(a => a.UpdateAsync(It.Is<List<ApplicationReviewFeedback>>(a => a.All(r => r.Status == ApplicationStatus.InReview.ToString()))));

            Assert.True(response.Success);
            Assert.NotNull(response.Value);
            Assert.NotNull(response.Value.Notifications);

            var notifications = response.Value.Notifications;
            Assert.Single(notifications);

            var notification = notifications[0];
            Assert.Equal(EmailTemplateNames.QFASTApplicationSubmittedNotification, notification.TemplateName);
            Assert.Equal(NotificationRecipientKind.QfauMailbox, notification.RecipientKind);
        });
    }


    [Fact]
    public async Task Test_Exception_Thrown_For_Locked_Application()
    {
        // Arrange
        var application = new Data.Entities.Application.Application()
        {
            Id = Guid.NewGuid(),
            Submitted = true
        };

        var summary = new View_SectionSummaryForApplication()
        {
            RemainingPages = 0
        };

        _applicationRepository.Setup(a => a.GetByIdAsync(application.Id)).Returns(Task.FromResult(application));
        _applicationRepository.Setup(a => a.GetSectionSummaryByApplicationIdAsync(application.Id)).ReturnsAsync([summary]);


        // Act
        var response = await _submitApplicationCommandHandler.Handle(new()
        {
            ApplicationId = application.Id,
        }, default);

        // Assert
        Assert.False(response.Success);
        Assert.IsAssignableFrom<RecordLockedException>(response.InnerException);
    }

    [Fact]
    public async Task Test_Exception_Thrown_For_Incomplete_Application()
    {
        // Arrange
        var application = new Data.Entities.Application.Application()
        {
            Id = Guid.NewGuid(),
            Submitted = false
        };

        var summary = new View_SectionSummaryForApplication()
        {
            RemainingPages = 1
        };

        _applicationRepository.Setup(a => a.GetByIdAsync(application.Id)).Returns(Task.FromResult(application));
        _applicationRepository.Setup(a => a.GetSectionSummaryByApplicationIdAsync(application.Id)).ReturnsAsync([summary]);


        // Act
        var response = await _submitApplicationCommandHandler.Handle(new()
        {
            ApplicationId = application.Id,
        }, default);

        // Assert
        Assert.False(response.Success);
        Assert.IsAssignableFrom<InvalidOperationException>(response.InnerException);
    }
}


