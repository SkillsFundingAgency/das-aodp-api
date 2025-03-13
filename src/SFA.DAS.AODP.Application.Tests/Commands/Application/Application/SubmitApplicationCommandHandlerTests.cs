using Moq;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Application;

public class SubmitApplicationCommandHandlerTests
{
    private readonly Mock<IApplicationRepository> _applicationRepository = new();
    private readonly Mock<IApplicationReviewRepository> _applicationReviewRepository = new();
    private readonly Mock<IApplicationReviewFeedbackRepository> _applicationReviewFeedbackRepository = new();
    private readonly SubmitApplicationCommandHandler _submitApplicationCommandHandler;

    public SubmitApplicationCommandHandlerTests()
    {
        _submitApplicationCommandHandler = new(_applicationRepository.Object, _applicationReviewRepository.Object, _applicationReviewFeedbackRepository.Object);
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
    public async Task Test_Application_Submitted_And_Review_Feedback_Updated()
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
        _applicationReviewFeedbackRepository.Verify(a => a.UpdateAsync(It.Is<List<ApplicationReviewFeedback>>(a => a.All(r => r.NewMessage))));
        _applicationReviewFeedbackRepository.Verify(a => a.UpdateAsync(It.Is<List<ApplicationReviewFeedback>>(a => a.All(r => r.Status == ApplicationStatus.InReview.ToString()))));
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


