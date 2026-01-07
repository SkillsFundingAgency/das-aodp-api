using Moq;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Commands.Application;
using SFA.DAS.AODP.Application.Queries.Application.Application;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
using SFA.DAS.AODP.Models.Application;
using SFA.DAS.AODP.Models.Form;
using SFA.DAS.AODP.Models.Validation;
using Xunit;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Application;

public class CreateApplicationCommandHandlerTests
{
    private readonly Mock<IApplicationRepository> _applicationRepository = new();
    private readonly Mock<IFormVersionRepository> _formVersionRepository = new();
    private readonly Mock<IQanValidationService> _qanValidationService = new();
    private readonly CreateApplicationCommandHandler _handler;

    private static readonly Guid FormVersionId = Guid.NewGuid();
    private static readonly Guid OrganisationId = Guid.NewGuid();
    private static readonly Guid CreatedApplicationId = Guid.NewGuid();

    private const string Title = "Level 3 Diploma in Engineering";
    private const string Owner = "Test Owner";
    private const string OrganisationName = "Pearson";
    private const string OrganisationUkprn = "12345678";
    private const string Qan = "10005122";
    private const string DefaultInvalidQanMessage = "Invalid QAN";
    private const string ExceptionMessage = "Create failed";

    public CreateApplicationCommandHandlerTests()
    {
        _handler = new CreateApplicationCommandHandler(
            _applicationRepository.Object,
            _formVersionRepository.Object,
            _qanValidationService.Object);
    }

    [Fact]
    public async Task Handle_PublishedFormVersion_ValidQan_CreatesApplication()
    {
        var formVersion = new Data.Entities.FormBuilder.FormVersion
        {
            Id = FormVersionId,
            Status = FormVersionStatus.Published.ToString()
        };

        var command = new CreateApplicationCommand
        {
            FormVersionId = FormVersionId,
            Title = Title,
            Owner = Owner,
            OrganisationId = OrganisationId,
            OrganisationName = OrganisationName,
            OrganisationUkprn = OrganisationUkprn,
            QualificationNumber = Qan
        };

        _formVersionRepository
            .Setup(r => r.GetFormVersionByIdAsync(FormVersionId))
            .ReturnsAsync(formVersion);

        _qanValidationService
            .Setup(v => v.ValidateAsync(Qan, Title, OrganisationName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QanValidationResult
            {
                IsValid = true
            });

        var createdApplication = new Data.Entities.Application.Application
        {
            Id = CreatedApplicationId
        };

        _applicationRepository
            .Setup(r => r.Create(It.Is<Data.Entities.Application.Application>(a =>
                a.FormVersionId == FormVersionId &&
                a.Name == Title &&
                a.Owner == Owner &&
                a.OrganisationId == OrganisationId &&
                a.QualificationNumber == Qan &&
                a.AwardingOrganisationName == OrganisationName &&
                a.AwardingOrganisationUkprn == OrganisationUkprn &&
                a.Status == ApplicationStatus.Draft.ToString())))
            .ReturnsAsync(createdApplication);

        var response = await _handler.Handle(command, default);

        Assert.Multiple(() =>
        {
            Assert.True(response.Success);
            Assert.NotNull(response.Value);
            Assert.Equal(CreatedApplicationId, response.Value.Id);

            _formVersionRepository.Verify(r =>
                r.GetFormVersionByIdAsync(FormVersionId), Times.Once);

            _qanValidationService.Verify(v =>
                v.ValidateAsync(Qan, Title, OrganisationName, It.IsAny<CancellationToken>()),
                Times.Once);

            _applicationRepository.Verify(r =>
                r.Create(It.IsAny<Data.Entities.Application.Application>()), Times.Once);
        });
    }

    [Fact]
    public async Task Handle_PublishedFormVersion_NoQan_DoesNotCallValidation_AndCreatesApplication()
    {
        var formVersion = new Data.Entities.FormBuilder.FormVersion
        {
            Id = FormVersionId,
            Status = FormVersionStatus.Published.ToString()
        };

        var command = new CreateApplicationCommand
        {
            FormVersionId = FormVersionId,
            Title = Title,
            Owner = Owner,
            OrganisationId = OrganisationId,
            OrganisationName = OrganisationName,
            OrganisationUkprn = OrganisationUkprn,
            QualificationNumber = null
        };

        _formVersionRepository
            .Setup(r => r.GetFormVersionByIdAsync(FormVersionId))
            .ReturnsAsync(formVersion);

        var createdApplication = new Data.Entities.Application.Application
        {
            Id = CreatedApplicationId
        };

        _applicationRepository
            .Setup(r => r.Create(It.Is<Data.Entities.Application.Application>(a =>
                a.FormVersionId == FormVersionId &&
                a.Name == Title &&
                a.Owner == Owner &&
                a.OrganisationId == OrganisationId &&
                a.QualificationNumber == null &&
                a.AwardingOrganisationName == OrganisationName &&
                a.AwardingOrganisationUkprn == OrganisationUkprn &&
                a.Status == ApplicationStatus.Draft.ToString())))
            .ReturnsAsync(createdApplication);

        var response = await _handler.Handle(command, default);

        Assert.Multiple(() =>
        {
            Assert.True(response.Success);
            Assert.NotNull(response.Value);
            Assert.Equal(CreatedApplicationId, response.Value.Id);

            _qanValidationService.Verify(v =>
                v.ValidateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _applicationRepository.Verify(r =>
                r.Create(It.IsAny<Data.Entities.Application.Application>()), Times.Once);
        });
    }

    [Fact]
    public async Task Handle_InvalidQan_ReturnsError_AndDoesNotCreate()
    {
        var formVersion = new Data.Entities.FormBuilder.FormVersion
        {
            Id = FormVersionId,
            Status = FormVersionStatus.Published.ToString()
        };

        var command = new CreateApplicationCommand
        {
            FormVersionId = FormVersionId,
            Title = Title,
            Owner = Owner,
            OrganisationId = OrganisationId,
            OrganisationName = OrganisationName,
            OrganisationUkprn = OrganisationUkprn,
            QualificationNumber = Qan
        };

        _formVersionRepository
            .Setup(r => r.GetFormVersionByIdAsync(FormVersionId))
            .ReturnsAsync(formVersion);

        _qanValidationService
            .Setup(v => v.ValidateAsync(Qan, Title, OrganisationName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QanValidationResult
            {
                IsValid = false,
                ValidationMessage = QanValidationMessages.TitleMismatch
            });

        var response = await _handler.Handle(command, default);

        Assert.Multiple(() =>
        {
            Assert.True(response.Success);
            Assert.NotNull(response.Value);
            Assert.False(response.Value.IsQanValid);
            Assert.Equal(QanValidationMessages.TitleMismatch, response.Value.QanValidationMessage);

            _qanValidationService.Verify(v =>
                v.ValidateAsync(Qan, Title, OrganisationName, It.IsAny<CancellationToken>()),
                Times.Once);

            _applicationRepository.Verify(r =>
                r.Create(It.IsAny<Data.Entities.Application.Application>()),
                Times.Never);
        });
    }

    [Fact]
    public async Task Handle_InvalidQan_NullMessage_UsesDefaultMessage_AndDoesNotCreate()
    {
        var formVersion = new Data.Entities.FormBuilder.FormVersion
        {
            Id = FormVersionId,
            Status = FormVersionStatus.Published.ToString()
        };

        var command = new CreateApplicationCommand
        {
            FormVersionId = FormVersionId,
            Title = Title,
            Owner = Owner,
            OrganisationId = OrganisationId,
            OrganisationName = OrganisationName,
            OrganisationUkprn = OrganisationUkprn,
            QualificationNumber = Qan
        };

        _formVersionRepository
            .Setup(r => r.GetFormVersionByIdAsync(FormVersionId))
            .ReturnsAsync(formVersion);

        _qanValidationService
            .Setup(v => v.ValidateAsync(Qan, Title, OrganisationName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QanValidationResult
            {
                IsValid = false,
                ValidationMessage = null
            });

        var response = await _handler.Handle(command, default);

        Assert.Multiple(() =>
        {
            Assert.True(response.Success);
            Assert.NotNull(response.Value);
            Assert.False(response.Value.IsQanValid);
            Assert.Equal(DefaultInvalidQanMessage, response.Value.QanValidationMessage);

            _qanValidationService.Verify(v =>
                v.ValidateAsync(Qan, Title, OrganisationName, It.IsAny<CancellationToken>()),
                Times.Once);

            _applicationRepository.Verify(r =>
                r.Create(It.IsAny<Data.Entities.Application.Application>()),
                Times.Never);
        });
    }

    [Fact]
    public async Task Handle_FormVersionNotPublished_ReturnsError_AndSkipsValidationAndCreate()
    {
        var formVersion = new Data.Entities.FormBuilder.FormVersion
        {
            Id = FormVersionId,
            Status = FormVersionStatus.Draft.ToString()
        };

        var command = new CreateApplicationCommand
        {
            FormVersionId = FormVersionId,
            Title = Title,
            Owner = Owner,
            OrganisationId = OrganisationId,
            OrganisationName = OrganisationName,
            OrganisationUkprn = OrganisationUkprn,
            QualificationNumber = Qan
        };

        _formVersionRepository
            .Setup(r => r.GetFormVersionByIdAsync(FormVersionId))
            .ReturnsAsync(formVersion);

        var response = await _handler.Handle(command, default);

        Assert.Multiple(() =>
        {
            Assert.False(response.Success);
            Assert.IsType<InvalidOperationException>(response.InnerException);
            Assert.Equal("The FormVersion is not published", response.ErrorMessage);

            _qanValidationService.Verify(v =>
                v.ValidateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _applicationRepository.Verify(r =>
                r.Create(It.IsAny<Data.Entities.Application.Application>()),
                Times.Never);
        });
    }

    [Fact]
    public async Task Handle_CreateThrows_ReturnsError_AndPopulatesExceptionDetails()
    {
        var formVersion = new Data.Entities.FormBuilder.FormVersion
        {
            Id = FormVersionId,
            Status = FormVersionStatus.Published.ToString()
        };

        var command = new CreateApplicationCommand
        {
            FormVersionId = FormVersionId,
            Title = Title,
            Owner = Owner,
            OrganisationId = OrganisationId,
            OrganisationName = OrganisationName,
            OrganisationUkprn = OrganisationUkprn,
            QualificationNumber = Qan
        };

        _formVersionRepository
            .Setup(r => r.GetFormVersionByIdAsync(FormVersionId))
            .ReturnsAsync(formVersion);

        _qanValidationService
            .Setup(v => v.ValidateAsync(Qan, Title, OrganisationName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QanValidationResult
            {
                IsValid = true
            });

        _applicationRepository
            .Setup(r => r.Create(It.IsAny<Data.Entities.Application.Application>()))
            .ThrowsAsync(new Exception(ExceptionMessage));

        var response = await _handler.Handle(command, default);

        Assert.Multiple(() =>
        {
            Assert.False(response.Success);
            Assert.NotNull(response.InnerException);
            Assert.Equal(ExceptionMessage, response.ErrorMessage);

            _formVersionRepository.Verify(r =>
                r.GetFormVersionByIdAsync(FormVersionId), Times.Once);

            _qanValidationService.Verify(v =>
                v.ValidateAsync(Qan, Title, OrganisationName, It.IsAny<CancellationToken>()),
                Times.Once);

            _applicationRepository.Verify(r =>
                r.Create(It.IsAny<Data.Entities.Application.Application>()), Times.Once);
        });
    }
}
