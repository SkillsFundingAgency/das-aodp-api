using Moq;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
using SFA.DAS.AODP.Models.Validation;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Application;

public class EditApplicationCommandHandlerTests
{
    private readonly Mock<IApplicationRepository> _applicationRepository = new();
    private readonly Mock<IQanValidationService> _qanValidationService = new();
    private readonly EditApplicationCommandHandler _handler;

    private static readonly Guid ApplicationId = Guid.NewGuid();

    private const string Title = "Test title";
    private const string Owner = "Test owner";

    private const string OldName = "Old name";
    private const string OldOwner = "Old owner";

    private const string OrganisationName = "Org name";

    private const string OriginalQan = "QAN1";
    private const string NewQan = "QAN2";
    private const string ExceptionMessage = "Database failure";
    private const string DefaultInvalidQanMessage = "Invalid QAN";

    public EditApplicationCommandHandlerTests()
    {
        _handler = new EditApplicationCommandHandler(_applicationRepository.Object, _qanValidationService.Object);
    }

    [Fact]
    public async Task Handle_QanNotChanged_DoesNotCallValidation_AndUpdatesApplication()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Submitted = false,
            Name = OldName,
            QualificationNumber = OriginalQan,
            Owner = OldOwner
        };

        var request = new EditApplicationCommand
        {
            ApplicationId = ApplicationId,
            QualificationNumber = OriginalQan,
            Title = Title,
            Owner = Owner
        };

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        _applicationRepository
            .Setup(r => r.UpdateAsync(It.Is<Data.Entities.Application.Application>(a =>
                a.Id == ApplicationId &&
                a.Name == Title &&
                a.QualificationNumber == OriginalQan &&
                a.Owner == Owner)))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(request, default);

        Assert.Multiple(() =>
        {
            Assert.True(result.Success);

            _qanValidationService.Verify(v =>
                v.ValidateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _applicationRepository.Verify(r =>
                r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()),
                Times.Once);
        });
    }

    [Fact]
    public async Task Handle_QanChanged_AndValidationPasses_UpdatesApplication()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Submitted = false,
            Name = OldName,
            QualificationNumber = OriginalQan,
            Owner = OldOwner,
            AwardingOrganisationName = OrganisationName
        };

        var request = new EditApplicationCommand
        {
            ApplicationId = ApplicationId,
            QualificationNumber = NewQan,
            Title = Title,
            Owner = Owner
        };

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        _qanValidationService
            .Setup(v => v.ValidateAsync(NewQan, Title, OrganisationName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QanValidationResult
            {
                IsValid = true
            });

        _applicationRepository
            .Setup(r => r.UpdateAsync(It.Is<Data.Entities.Application.Application>(a =>
                a.Id == ApplicationId &&
                a.Name == Title &&
                a.QualificationNumber == NewQan &&
                a.Owner == Owner)))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(request, default);

        Assert.Multiple(() =>
        {
            Assert.True(result.Success);

            _qanValidationService.Verify(v =>
                v.ValidateAsync(NewQan, Title, OrganisationName, It.IsAny<CancellationToken>()),
                Times.Once);

            _applicationRepository.Verify(r =>
                r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()),
                Times.Once);
        });
    }

    [Fact]
    public async Task Handle_QanChanged_AndValidationFails_DoesNotUpdateApplication_AndReturnsError()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Submitted = false,
            Name = OldName,
            QualificationNumber = OriginalQan,
            AwardingOrganisationName = OrganisationName,
            Owner = OldOwner
        };

        var request = new EditApplicationCommand
        {
            ApplicationId = ApplicationId,
            QualificationNumber = NewQan,
            Title = Title,
            Owner = Owner
        };

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        _qanValidationService
            .Setup(v => v.ValidateAsync(NewQan, Title, OrganisationName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QanValidationResult
            {
                IsValid = false,
                ValidationMessage = QanValidationMessages.TitleMismatch
            });

        var result = await _handler.Handle(request, default);

        Assert.Multiple(() =>
        {
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.False(result.Value.IsQanValid);
            Assert.Equal(QanValidationMessages.TitleMismatch, result.Value.QanValidationMessage);

            _applicationRepository.Verify(r =>
                r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()),
                Times.Never);
        });
    }

    [Fact]
    public async Task Handle_QanChanged_AndValidationFails_WithNullMessage_UsesDefaultMessage()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Submitted = false,
            Name = OldName,
            QualificationNumber = OriginalQan,
            AwardingOrganisationName = OrganisationName,
            Owner = OldOwner
        };

        var request = new EditApplicationCommand
        {
            ApplicationId = ApplicationId,
            QualificationNumber = NewQan,
            Title = Title,
            Owner = Owner
        };

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        _qanValidationService
            .Setup(v => v.ValidateAsync(NewQan, Title, OrganisationName, It.IsAny<CancellationToken>()))
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
        });
    }

    [Fact]
    public async Task Handle_ApplicationSubmitted_ReturnsRecordLockedException_AndDoesNotValidateOrUpdate()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Submitted = true,
            Name = OldName,
            QualificationNumber = OriginalQan,
            AwardingOrganisationName = OrganisationName,
            Owner = OldOwner
        };

        var request = new EditApplicationCommand
        {
            ApplicationId = ApplicationId,
            QualificationNumber = NewQan,
            Title = Title,
            Owner = Owner
        };

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        var result = await _handler.Handle(request, default);

        Assert.Multiple(() =>
        {
            Assert.False(result.Success);
            Assert.NotNull(result.InnerException);
            Assert.IsType<RecordLockedException>(result.InnerException);

            _qanValidationService.Verify(v =>
                v.ValidateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _applicationRepository.Verify(r =>
                r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()),
                Times.Never);
        });
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ReturnsError_AndPopulatesExceptionDetails()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Submitted = false,
            Name = OldName,
            QualificationNumber = OriginalQan,
            Owner = OldOwner
        };

        var request = new EditApplicationCommand
        {
            ApplicationId = ApplicationId,
            QualificationNumber = OriginalQan,
            Title = Title,
            Owner = Owner
        };

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        _applicationRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()))
            .ThrowsAsync(new Exception(ExceptionMessage));

        var result = await _handler.Handle(request, default);

        Assert.Multiple(() =>
        {
            Assert.False(result.Success);
            Assert.NotNull(result.InnerException);
            Assert.Equal(ExceptionMessage, result.ErrorMessage);

            _applicationRepository.Verify(r =>
                r.GetByIdAsync(ApplicationId), Times.Once);

            _applicationRepository.Verify(r =>
                r.UpdateAsync(It.IsAny<Data.Entities.Application.Application>()), Times.Once);
        });
    }
}
