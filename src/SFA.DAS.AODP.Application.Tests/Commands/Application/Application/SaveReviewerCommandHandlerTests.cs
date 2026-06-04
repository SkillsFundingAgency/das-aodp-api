using MediatR;
using Moq;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Application.Commands.Application.Review;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Application;

public class SaveReviewerCommandHandlerTests
{
    private readonly Mock<IApplicationRepository> _repository = new();
    private readonly Mock<IMediator> _mediator = new();
    private readonly SaveReviewerCommandHandler _handler;

    private static readonly Guid ApplicationId = Guid.NewGuid();

    private const string Reviewer1Field = "Reviewer1";
    private const string Reviewer2Field = "Reviewer2";

    private const string PreviousReviewer1 = "Old1";
    private const string PreviousReviewer2 = "Old2";

    private const string NewReviewer = "NewReviewer";
    private const string DuplicateReviewer = "Bob";
    private const string InvalidUserType = "NotReal";

    private const string UserTypeValue = "Qfau";
    private const string SentByName = "Test User";
    private const string SentByEmail = "user@test.com";

    private const string MessageFailure = "FAIL";
    private const string NotFoundText = "not found";

    public SaveReviewerCommandHandlerTests()
    {
        _handler = new SaveReviewerCommandHandler(_repository.Object, _mediator.Object);

        _mediator.Setup(m => m.Send(
            It.IsAny<CreateApplicationMessageCommand>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse>
        {
            Success = true,
            Value = new CreateApplicationMessageCommandResponse()
        });
    }

    [Fact]
    public async Task Handle_ReviewerNotChanged_ReturnsSuccess_NoUpdate_NoMessage()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Reviewer1 = PreviousReviewer1
        };

        _repository.Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        var result = await _handler.Handle(new SaveReviewerCommand
        {
            ApplicationId = ApplicationId,
            ReviewerFieldName = Reviewer1Field,
            ReviewerValue = PreviousReviewer1,
            SentByEmail = SentByEmail,
            SentByName = SentByName,
            UserType = UserTypeValue
        }, default);

        Assert.Multiple(() =>
        {
            Assert.True(result.Success);
            _repository.Verify(r => r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()), Times.Never);
            _mediator.Verify(r => r.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        });
    }

    [Fact]
    public async Task Handle_DuplicateReviewer_CaseInsensitive_ReturnsDuplicateError()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Reviewer1 = DuplicateReviewer,
            Reviewer2 = PreviousReviewer2
        };

        _repository.Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        var result = await _handler.Handle(new SaveReviewerCommand
        {
            ApplicationId = ApplicationId,
            ReviewerFieldName = Reviewer2Field,
            ReviewerValue = "  bob  ",
            SentByEmail = SentByEmail,
            SentByName = SentByName,
            UserType = UserTypeValue
        }, default);

        Assert.Multiple(() =>
        {
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.True(result.Value.DuplicateReviewerError);
            _repository.Verify(r => r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()), Times.Never);
            _mediator.Verify(r => r.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        });
    }

    [Fact]
    public async Task Handle_ValidReviewer1_Updates_AndSendsMessage()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Reviewer1 = PreviousReviewer1,
            Reviewer2 = PreviousReviewer2
        };

        _repository.Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        var result = await _handler.Handle(new SaveReviewerCommand
        {
            ApplicationId = ApplicationId,
            ReviewerFieldName = Reviewer1Field,
            ReviewerValue = NewReviewer,
            SentByEmail = SentByEmail,
            SentByName = SentByName,
            UserType = UserTypeValue
        }, default);

        Assert.Multiple(() =>
        {
            Assert.True(result.Success);

            _repository.Verify(r => r.UpdateAsync(It.Is<Data.Entities.Application.Application>(
                a => a.Reviewer1 == NewReviewer &&
                     a.Reviewer2 == PreviousReviewer2
            )), Times.Once);

            _mediator.Verify(m => m.Send(It.Is<CreateApplicationMessageCommand>(c =>
                c.ApplicationId == ApplicationId &&
                c.MessageText.Contains($"Previous {Reviewer1Field}: {PreviousReviewer1}") &&
                c.MessageText.Contains($"New {Reviewer1Field}: {NewReviewer}")
            ), It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    [Fact]
    public async Task Handle_RecordNotFound_ReturnsErrorResponse()
    {
        _repository.Setup(r => r.GetByIdAsync(ApplicationId))
            .ThrowsAsync(new RecordNotFoundException(ApplicationId));

        var result = await _handler.Handle(new SaveReviewerCommand
        {
            ApplicationId = ApplicationId,
            ReviewerFieldName = Reviewer1Field,
            ReviewerValue = NewReviewer,
            SentByEmail = SentByEmail,
            SentByName = SentByName,
            UserType = UserTypeValue
        }, default);

        Assert.Multiple(() =>
        {
            Assert.False(result.Success);
            Assert.Equal($"Application {ApplicationId} not found.", result.ErrorMessage);
            _mediator.Verify(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        });
    }

    [Fact]
    public async Task Handle_InvalidUserType_ReturnsErrorResponse()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Reviewer1 = PreviousReviewer1
        };

        _repository.Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        var result = await _handler.Handle(new SaveReviewerCommand
        {
            ApplicationId = ApplicationId,
            ReviewerFieldName = Reviewer1Field,
            ReviewerValue = NewReviewer,
            SentByEmail = SentByEmail,
            SentByName = SentByName,
            UserType = InvalidUserType
        }, default);

        Assert.Multiple(() =>
        {
            Assert.False(result.Success);
            Assert.IsType<ArgumentException>(result.InnerException);
            _repository.Verify(r => r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()), Times.Never);
            _mediator.Verify(r => r.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        });
    }

    [Fact]
    public async Task Handle_MessageFailure_ReturnsErrorResponse()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Reviewer1 = PreviousReviewer1
        };

        _repository.Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        _mediator.Setup(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse>
            {
                Success = false,
                ErrorMessage = MessageFailure
            });

        var result = await _handler.Handle(new SaveReviewerCommand
        {
            ApplicationId = ApplicationId,
            ReviewerFieldName = Reviewer1Field,
            ReviewerValue = NewReviewer,
            SentByEmail = SentByEmail,
            SentByName = SentByName,
            UserType = UserTypeValue
        }, default);

        Assert.Multiple(() =>
        {
            Assert.False(result.Success);
            Assert.Equal(MessageFailure, result.ErrorMessage);
            _repository.Verify(r => r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()), Times.Once);
        });
    }
}