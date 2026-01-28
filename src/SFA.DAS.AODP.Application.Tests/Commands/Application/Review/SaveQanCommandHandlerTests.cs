using MediatR;
using Moq;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Application.Commands.Application.Review;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
using SFA.DAS.AODP.Models.Application;
using SFA.DAS.AODP.Models.Validation;
using Xunit;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Review;

public class SaveQanCommandHandlerTests
{
    private readonly Mock<IApplicationRepository> _applicationRepository = new();
    private readonly Mock<IApplicationReviewRepository> _reviewRepository = new();
    private readonly Mock<IMediator> _mediator = new();
    private readonly Mock<IQanValidationService> _qanValidationService = new();
    private readonly SaveQanCommandHandler _handler;

    private static readonly Guid ApplicationId = Guid.NewGuid();
    private static readonly Guid ApplicationReviewId = Guid.NewGuid();

    private const string OldQan = "QAN1";
    private const string NewQan = "QAN2";
    private const string ApplicationName = "Level 3 Diploma in Engineering";
    private const string AwardingOrganisationName = "Pearson";
    private const string DefaultInvalidQanMessage = "Invalid QAN";
    private const string QanValidationMessage = "QAN mismatch";
    private const string SentByEmail = "sender@test.com";
    private const string SentByName = "Sender";
    private const string MediatorErrorMessage = "message failed";
    private const string UpdateExceptionMessage = "update failed";

    public SaveQanCommandHandlerTests()
    {
        _handler = new SaveQanCommandHandler(
            _applicationRepository.Object,
            _mediator.Object,
            _reviewRepository.Object,
            _qanValidationService.Object);
    }

    [Fact]
    public async Task Handle_QanNotChanged_DoesNotValidate_UpdatesApplication_SendsMessage_ReturnsSuccess()
    {
        var review = new Data.Entities.Application.ApplicationReview
        {
            Id = ApplicationReviewId,
            ApplicationId = ApplicationId
        };

        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Name = ApplicationName,
            AwardingOrganisationName = AwardingOrganisationName,
            QualificationNumber = OldQan
        };

        var request = new SaveQanCommand
        {
            ApplicationReviewId = ApplicationReviewId,
            Qan = OldQan,
            SentByEmail = SentByEmail,
            SentByName = SentByName
        };

        _reviewRepository
            .Setup(r => r.GetByIdAsync(ApplicationReviewId))
            .ReturnsAsync(review);

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        _applicationRepository
            .Setup(r => r.UpdateAsync(It.Is<Data.Entities.Application.Application>(a =>
                a.Id == ApplicationId &&
                a.QualificationNumber == OldQan)))
            .Returns(Task.CompletedTask);

        var msgWrapper = new BaseMediatrResponse<CreateApplicationMessageCommandResponse>
        {
            Success = true,
            Value = new CreateApplicationMessageCommandResponse { Id = Guid.NewGuid() }
        };

        _mediator
            .Setup(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(msgWrapper));

        var result = await _handler.Handle(request, default);

        Assert.Multiple(() =>
        {
            Assert.True(result.Success);
            Assert.NotNull(result.Value);

            _qanValidationService.Verify(v =>
                v.ValidateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _applicationRepository.Verify(r =>
                r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()),
                Times.Once);

            _mediator.Verify(m =>
                m.Send(It.Is<CreateApplicationMessageCommand>(c =>
                        c.ApplicationId == ApplicationId &&
                        c.SentByEmail == SentByEmail &&
                        c.SentByName == SentByName &&
                        c.UserType == UserType.Qfau.ToString() &&
                        c.MessageType == MessageType.QanUpdated.ToString() &&
                        c.MessageText.Contains($"Previous QAN: {OldQan}") &&
                        c.MessageText.Contains($"New QAN: {OldQan}")),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        });
    }

    [Fact]
    public async Task Handle_QanChanged_ValidationPasses_UpdatesApplication_SendsMessage_ReturnsSuccess()
    {
        var review = new Data.Entities.Application.ApplicationReview
        {
            Id = ApplicationReviewId,
            ApplicationId = ApplicationId
        };

        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Name = ApplicationName,
            AwardingOrganisationName = AwardingOrganisationName,
            QualificationNumber = OldQan
        };

        var request = new SaveQanCommand
        {
            ApplicationReviewId = ApplicationReviewId,
            Qan = NewQan,
            SentByEmail = SentByEmail,
            SentByName = SentByName
        };

        _reviewRepository
            .Setup(r => r.GetByIdAsync(ApplicationReviewId))
            .ReturnsAsync(review);

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        _qanValidationService
            .Setup(v => v.ValidateAsync(NewQan, ApplicationName, AwardingOrganisationName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QanValidationResult { IsValid = true });

        _applicationRepository
            .Setup(r => r.UpdateAsync(It.Is<Data.Entities.Application.Application>(a =>
                a.Id == ApplicationId &&
                a.QualificationNumber == NewQan)))
            .Returns(Task.CompletedTask);

        var msgWrapper = new BaseMediatrResponse<CreateApplicationMessageCommandResponse>
        {
            Success = true,
            Value = new CreateApplicationMessageCommandResponse { Id = Guid.NewGuid() }
        };

        _mediator
            .Setup(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(msgWrapper));

        var result = await _handler.Handle(request, default);

        Assert.Multiple(() =>
        {
            Assert.True(result.Success);
            Assert.NotNull(result.Value);

            _qanValidationService.Verify(v =>
                v.ValidateAsync(NewQan, ApplicationName, AwardingOrganisationName, It.IsAny<CancellationToken>()),
                Times.Once);

            _applicationRepository.Verify(r =>
                r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()),
                Times.Once);

            _mediator.Verify(m =>
                m.Send(It.Is<CreateApplicationMessageCommand>(c =>
                        c.ApplicationId == ApplicationId &&
                        c.SentByEmail == SentByEmail &&
                        c.SentByName == SentByName &&
                        c.UserType == UserType.Qfau.ToString() &&
                        c.MessageType == MessageType.QanUpdated.ToString() &&
                        c.MessageText.Contains($"Previous QAN: {OldQan}") &&
                        c.MessageText.Contains($"New QAN: {NewQan}")),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        });
    }

    [Fact]
    public async Task Handle_QanChanged_ValidationFails_ReturnsSuccess_WithInvalidQan_AndDoesNotUpdateOrSendMessage()
    {
        var review = new Data.Entities.Application.ApplicationReview
        {
            Id = ApplicationReviewId,
            ApplicationId = ApplicationId
        };

        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Name = ApplicationName,
            AwardingOrganisationName = AwardingOrganisationName,
            QualificationNumber = OldQan
        };

        var request = new SaveQanCommand
        {
            ApplicationReviewId = ApplicationReviewId,
            Qan = NewQan,
            SentByEmail = SentByEmail,
            SentByName = SentByName
        };

        _reviewRepository
            .Setup(r => r.GetByIdAsync(ApplicationReviewId))
            .ReturnsAsync(review);

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        _qanValidationService
            .Setup(v => v.ValidateAsync(NewQan, ApplicationName, AwardingOrganisationName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QanValidationResult
            {
                IsValid = false,
                ValidationMessage = QanValidationMessage
            });

        var result = await _handler.Handle(request, default);

        Assert.Multiple(() =>
        {
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.False(result.Value.IsQanValid);
            Assert.Equal(QanValidationMessage, result.Value.QanValidationMessage);

            _applicationRepository.Verify(r =>
                r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()),
                Times.Never);

            _mediator.Verify(m =>
                m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        });
    }

    [Fact]
    public async Task Handle_QanChanged_ValidationFails_NullMessage_UsesDefaultMessage_AndDoesNotUpdateOrSendMessage()
    {
        var review = new Data.Entities.Application.ApplicationReview
        {
            Id = ApplicationReviewId,
            ApplicationId = ApplicationId
        };

        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Name = ApplicationName,
            AwardingOrganisationName = AwardingOrganisationName,
            QualificationNumber = OldQan
        };

        var request = new SaveQanCommand
        {
            ApplicationReviewId = ApplicationReviewId,
            Qan = NewQan,
            SentByEmail = SentByEmail,
            SentByName = SentByName
        };

        _reviewRepository
            .Setup(r => r.GetByIdAsync(ApplicationReviewId))
            .ReturnsAsync(review);

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        _qanValidationService
            .Setup(v => v.ValidateAsync(NewQan, ApplicationName, AwardingOrganisationName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QanValidationResult
            {
                IsValid = false,
                ValidationMessage = null
            });

        var result = await _handler.Handle(request, default);

        Assert.Multiple(() =>
        {
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.False(result.Value.IsQanValid);
            Assert.Equal(DefaultInvalidQanMessage, result.Value.QanValidationMessage);

            _applicationRepository.Verify(r =>
                r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()),
                Times.Never);

            _mediator.Verify(m =>
                m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        });
    }

    [Fact]
    public async Task Handle_MediatorReturnsFailure_ReturnsError()
    {
        var review = new Data.Entities.Application.ApplicationReview
        {
            Id = ApplicationReviewId,
            ApplicationId = ApplicationId
        };

        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Name = ApplicationName,
            AwardingOrganisationName = AwardingOrganisationName,
            QualificationNumber = OldQan
        };

        var request = new SaveQanCommand
        {
            ApplicationReviewId = ApplicationReviewId,
            Qan = NewQan,
            SentByEmail = SentByEmail,
            SentByName = SentByName
        };

        _reviewRepository
            .Setup(r => r.GetByIdAsync(ApplicationReviewId))
            .ReturnsAsync(review);

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        _qanValidationService
            .Setup(v => v.ValidateAsync(NewQan, ApplicationName, AwardingOrganisationName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QanValidationResult { IsValid = true });

        _applicationRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()))
            .Returns(Task.CompletedTask);

        var msgWrapper = new BaseMediatrResponse<CreateApplicationMessageCommandResponse>
        {
            Success = false,
            ErrorMessage = MediatorErrorMessage,
            InnerException = new Exception(MediatorErrorMessage)
        };

        _mediator
            .Setup(m => m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(msgWrapper));

        var result = await _handler.Handle(request, default);

        Assert.Multiple(() =>
        {
            Assert.False(result.Success);
            Assert.Equal(MediatorErrorMessage, result.ErrorMessage);
            Assert.NotNull(result.InnerException);

            _mediator.Verify(m =>
                m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
        });
    }

    [Fact]
    public async Task Handle_UpdateThrows_ReturnsError_AndPopulatesExceptionDetails()
    {
        var review = new Data.Entities.Application.ApplicationReview
        {
            Id = ApplicationReviewId,
            ApplicationId = ApplicationId
        };

        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Name = ApplicationName,
            AwardingOrganisationName = AwardingOrganisationName,
            QualificationNumber = OldQan
        };

        var request = new SaveQanCommand
        {
            ApplicationReviewId = ApplicationReviewId,
            Qan = NewQan,
            SentByEmail = SentByEmail,
            SentByName = SentByName
        };

        _reviewRepository
            .Setup(r => r.GetByIdAsync(ApplicationReviewId))
            .ReturnsAsync(review);

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        _qanValidationService
            .Setup(v => v.ValidateAsync(NewQan, ApplicationName, AwardingOrganisationName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QanValidationResult { IsValid = true });

        _applicationRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()))
            .ThrowsAsync(new Exception(UpdateExceptionMessage));

        var result = await _handler.Handle(request, default);

        Assert.Multiple(() =>
        {
            Assert.False(result.Success);
            Assert.Equal(UpdateExceptionMessage, result.ErrorMessage);
            Assert.NotNull(result.InnerException);

            _applicationRepository.Verify(r =>
                r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()),
                Times.Once);

            _mediator.Verify(m =>
                m.Send(It.IsAny<CreateApplicationMessageCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        });
    }
}
