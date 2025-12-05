using MediatR;
using Moq;
using SFA.DAS.AODP.Application.Commands.Application;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Application;

public class WithdrawApplicationCommandHandlerTests
{
    private readonly Mock<IApplicationRepository> _applicationRepository = new();
    private readonly Mock<IMediator> _mediator = new();
    private readonly WithdrawApplicationCommandHandler _handler;

    private static readonly Guid ApplicationId = Guid.NewGuid();
    private const string WithdrawnBy = "Test User";
    private const string WithdrawnByEmail = "user@test.com";

    public WithdrawApplicationCommandHandlerTests()
    {
        _handler = new WithdrawApplicationCommandHandler(_applicationRepository.Object, _mediator.Object);

        var notifications = new List<NotificationDefinition>
        {
            new()
            {
                TemplateName = EmailTemplateNames.QFASTSubmittedApplicationChangedNotification,
                RecipientKind = NotificationRecipientKind.QfauMailbox
            }
        };

        _mediator
            .Setup(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()))
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
    public async Task Handle_ValidApplication_UpdatesStatus_AndCreatesMessage()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Submitted = true,
            Status = nameof(ApplicationStatus.InReview)
        };

        _applicationRepository.Setup(r => r.GetByIdAsync(ApplicationId)).ReturnsAsync(application);

        var response = await _handler.Handle(new WithdrawApplicationCommand
        {
            ApplicationId = ApplicationId,
            WithdrawnBy = WithdrawnBy,
            WithdrawnByEmail = WithdrawnByEmail
        }, default);

        Assert.Multiple(() =>
        {
            Assert.True(response.Success);
            Assert.NotNull(response.Value);
            Assert.NotNull(response.Value.Notifications);
            Assert.Single(response.Value.Notifications);

            _applicationRepository.Verify(r => r.UpdateAsync(It.Is<Data.Entities.Application.Application>(a =>
                a.Id == ApplicationId &&
                a.Status == ApplicationStatus.Withdrawn.ToString() &&
                a.WithdrawnBy == WithdrawnBy &&
                a.WithdrawnAt.HasValue)), Times.Once);

            _mediator.Verify(m => m.Send(It.Is<CreateApplicationMessageCommand>(c =>
                c.ApplicationId == ApplicationId &&
                c.MessageType == MessageType.ApplicationWithdrawn.ToString() &&
                c.SentByEmail == WithdrawnByEmail &&
                c.SentByName == WithdrawnBy), default), Times.Once);
        });
    }

    [Fact]
    public async Task Handle_AlreadyWithdrawnApplication_ThrowsRecordLockedException()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Status = nameof(ApplicationStatus.Withdrawn)
        };

        _applicationRepository.Setup(r => r.GetByIdAsync(ApplicationId)).ReturnsAsync(application);

        var response = await _handler.Handle(new WithdrawApplicationCommand
        {
            ApplicationId = ApplicationId,
            WithdrawnBy = WithdrawnBy,
            WithdrawnByEmail = WithdrawnByEmail
        }, default);

        Assert.Multiple(() =>
        {
            Assert.False(response.Success);
            Assert.IsAssignableFrom<RecordLockedException>(response.InnerException);
        });
    }

    [Fact]
    public async Task Handle_MessageCommandFails_ReturnsErrorResponse()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Submitted = true, 
            Status = nameof(ApplicationStatus.InReview)
        };

        _applicationRepository.Setup(r => r.GetByIdAsync(ApplicationId)).ReturnsAsync(application);

        const string InnerError = "Message send failed";

        _mediator
            .Setup(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BaseMediatrResponse<CreateApplicationMessageCommandResponse>
            {
                Success = false,
                ErrorMessage = InnerError
            });

        var response = await _handler.Handle(new WithdrawApplicationCommand
        {
            ApplicationId = ApplicationId,
            WithdrawnBy = WithdrawnBy,
            WithdrawnByEmail = WithdrawnByEmail
        }, default);

        Assert.Multiple(() =>
        {
            Assert.False(response.Success);
            Assert.NotNull(response.InnerException);
            Assert.Equal(InnerError, response.InnerException!.Message);
        });
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ReturnsErrorResponse()
    {
        const string ExceptionMessage = "Repository exception";

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ThrowsAsync(new InvalidOperationException(ExceptionMessage));

        var response = await _handler.Handle(new WithdrawApplicationCommand
        {
            ApplicationId = ApplicationId,
            WithdrawnBy = WithdrawnBy,
            WithdrawnByEmail = WithdrawnByEmail
        }, default);

        Assert.Multiple(() =>
        {
            Assert.False(response.Success);
            Assert.IsAssignableFrom<InvalidOperationException>(response.InnerException);
            Assert.Equal(ExceptionMessage, response.ErrorMessage);
        });
    }
}
