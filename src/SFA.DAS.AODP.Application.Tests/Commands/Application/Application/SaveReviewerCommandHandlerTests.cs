using MediatR;
using Moq;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Application.Commands.Application.Review;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Application;

public class SaveReviewerCommandHandlerTests
{
    private readonly Mock<IApplicationRepository> _repository = new();
    private readonly Mock<IMediator> _mediator = new();
    private readonly SaveReviewerCommandHandler _handler;

    private static readonly Guid ApplicationId = Guid.NewGuid();
    private const string SentByName = "Test User";
    private const string SentByEmail = "user@test.com";
    private const string UserTypeValue = "Qfau";
    private const string ReviewerFieldName1 = "Reviewer1";
    private const string ReviewerFieldName2 = "Reviewer2";
    private const string PreviousReviewer1 = "Old Reviewer 1";
    private const string PreviousReviewer2 = "Old Reviewer 2";
    private const string NewReviewer = "New Reviewer";
    private const string DuplicateReviewer = "Same Reviewer";
    private const string InvalidUserType = "NotAUserType";
    private const string InvalidReviewerFieldName = "Reviewer3";
    private const string MessageSendFailed = "Message send failed";
    private const string RepositoryExceptionMessage = "Repository exception";

    public SaveReviewerCommandHandlerTests()
    {
        _handler = new SaveReviewerCommandHandler(_repository.Object, _mediator.Object);

        _mediator
            .Setup(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse>
            {
                Success = true,
                Value = new CreateApplicationMessageCommandResponse()
            });
    }

    [Fact]
    public async Task Handle_ValidReviewer1_UpdatesApplication_AndCreatesMessage()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Reviewer1 = PreviousReviewer1,
            Reviewer2 = PreviousReviewer2
        };

        _repository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        var response = await _handler.Handle(new SaveReviewerCommand
        {
            ApplicationId = ApplicationId,
            ReviewerFieldName = ReviewerFieldName1,
            ReviewerValue = NewReviewer,
            SentByEmail = SentByEmail,
            SentByName = SentByName,
            UserType = UserTypeValue
        }, default);

        Assert.Multiple(() =>
        {
            Assert.True(response.Success);

            _repository.Verify(r => r.UpdateAsync(It.Is<Data.Entities.Application.Application>(a =>
                a.Id == ApplicationId &&
                a.Reviewer1 == NewReviewer &&
                a.Reviewer2 == PreviousReviewer2
            )), Times.Once);

            _mediator.Verify(m => m.Send(It.Is<CreateApplicationMessageCommand>(c =>
                c.ApplicationId == ApplicationId &&
                c.SentByEmail == SentByEmail &&
                c.SentByName == SentByName &&
                c.UserType == UserType.Qfau.ToString() &&
                c.MessageType == MessageType.QfauOwnerUpdated.ToString() &&
                c.MessageText.Contains($"Previous {ReviewerFieldName1}:") &&
                c.MessageText.Contains($"New {ReviewerFieldName1}:")
            ), It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    [Fact]
    public async Task Handle_ValidReviewer2_UpdatesApplication_AndCreatesMessage()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Reviewer1 = PreviousReviewer1,
            Reviewer2 = PreviousReviewer2
        };

        _repository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        var response = await _handler.Handle(new SaveReviewerCommand
        {
            ApplicationId = ApplicationId,
            ReviewerFieldName = ReviewerFieldName2,
            ReviewerValue = NewReviewer,
            SentByEmail = SentByEmail,
            SentByName = SentByName,
            UserType = UserTypeValue
        }, default);

        Assert.Multiple(() =>
        {
            Assert.True(response.Success);

            _repository.Verify(r => r.UpdateAsync(It.Is<Data.Entities.Application.Application>(a =>
                a.Id == ApplicationId &&
                a.Reviewer1 == PreviousReviewer1 &&
                a.Reviewer2 == NewReviewer
            )), Times.Once);

            _mediator.Verify(m => m.Send(It.Is<CreateApplicationMessageCommand>(c =>
                c.ApplicationId == ApplicationId &&
                c.SentByEmail == SentByEmail &&
                c.SentByName == SentByName &&
                c.UserType == UserType.Qfau.ToString() &&
                c.MessageType == MessageType.QfauOwnerUpdated.ToString() &&
                c.MessageText.Contains($"Previous {ReviewerFieldName2}:") &&
                c.MessageText.Contains($"New {ReviewerFieldName2}:")
            ), It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    [Fact]
    public async Task Handle_DuplicateReviewer_ReturnsDuplicateError_DoesNotUpdate_AndDoesNotSendMessage()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Reviewer1 = DuplicateReviewer,
            Reviewer2 = PreviousReviewer2
        };

        _repository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        var response = await _handler.Handle(new SaveReviewerCommand
        {
            ApplicationId = ApplicationId,
            ReviewerFieldName = ReviewerFieldName2,
            ReviewerValue = DuplicateReviewer,
            SentByEmail = SentByEmail,
            SentByName = SentByName,
            UserType = UserTypeValue
        }, default);

        Assert.Multiple(() =>
        {
            Assert.True(response.Success);
            Assert.NotNull(response.Value);
            Assert.True(response.Value.DuplicateReviewerError);

            _repository.Verify(r => r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()), Times.Never);
            _mediator.Verify(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        });
    }

    [Fact]
    public async Task Handle_InvalidUserType_ReturnsErrorResponse()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Reviewer1 = PreviousReviewer1,
            Reviewer2 = PreviousReviewer2
        };

        _repository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        var response = await _handler.Handle(new SaveReviewerCommand
        {
            ApplicationId = ApplicationId,
            ReviewerFieldName = ReviewerFieldName1,
            ReviewerValue = NewReviewer,
            SentByEmail = SentByEmail,
            SentByName = SentByName,
            UserType = InvalidUserType
        }, default);

        Assert.Multiple(() =>
        {
            Assert.False(response.Success);
            Assert.NotNull(response.InnerException);
            Assert.IsAssignableFrom<ArgumentException>(response.InnerException);

            _repository.Verify(r => r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()), Times.Never);
            _mediator.Verify(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        });
    }

    [Fact]
    public async Task Handle_InvalidReviewerFieldName_ReturnsErrorResponse()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Reviewer1 = PreviousReviewer1,
            Reviewer2 = PreviousReviewer2
        };

        _repository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        var response = await _handler.Handle(new SaveReviewerCommand
        {
            ApplicationId = ApplicationId,
            ReviewerFieldName = InvalidReviewerFieldName,
            ReviewerValue = NewReviewer,
            SentByEmail = SentByEmail,
            SentByName = SentByName,
            UserType = UserTypeValue
        }, default);

        Assert.Multiple(() =>
        {
            Assert.False(response.Success);
            Assert.NotNull(response.InnerException);
            Assert.IsAssignableFrom<ArgumentOutOfRangeException>(response.InnerException);

            _repository.Verify(r => r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()), Times.Never);
            _mediator.Verify(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        });
    }

    [Fact]
    public async Task Handle_MessageCommandFails_ReturnsErrorResponse()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Reviewer1 = PreviousReviewer1,
            Reviewer2 = PreviousReviewer2
        };

        _repository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        _mediator
            .Setup(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse>
            {
                Success = false,
                ErrorMessage = MessageSendFailed
            });

        var response = await _handler.Handle(new SaveReviewerCommand
        {
            ApplicationId = ApplicationId,
            ReviewerFieldName = ReviewerFieldName1,
            ReviewerValue = NewReviewer,
            SentByEmail = SentByEmail,
            SentByName = SentByName,
            UserType = UserTypeValue
        }, default);

        Assert.Multiple(() =>
        {
            Assert.False(response.Success);
            Assert.NotNull(response.InnerException);
            Assert.Equal(MessageSendFailed, response.InnerException!.Message);

            _repository.Verify(r => r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()), Times.Once);
        });
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ReturnsErrorResponse()
    {
        _repository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ThrowsAsync(new InvalidOperationException(RepositoryExceptionMessage));

        var response = await _handler.Handle(new SaveReviewerCommand
        {
            ApplicationId = ApplicationId,
            ReviewerFieldName = ReviewerFieldName1,
            ReviewerValue = NewReviewer,
            SentByEmail = SentByEmail,
            SentByName = SentByName,
            UserType = UserTypeValue
        }, default);

        Assert.Multiple(() =>
        {
            Assert.False(response.Success);
            Assert.IsAssignableFrom<InvalidOperationException>(response.InnerException);
            Assert.Equal(RepositoryExceptionMessage, response.ErrorMessage);

            _repository.Verify(r => r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()), Times.Never);
            _mediator.Verify(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        });
    }
}
