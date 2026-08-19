using Moq;
using SFA.DAS.AODP.Application.Commands.Application.Application;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Application;

public class DeleteApplicationCommandHandlerTests
{
    private readonly Mock<IApplicationRepository> _applicationRepository = new();
    private readonly DeleteApplicationCommandHandler _handler;

    private static readonly Guid ApplicationId = Guid.NewGuid();
    private const string ExceptionMessage = "Database failure";

    public DeleteApplicationCommandHandlerTests()
    {
        _handler = new DeleteApplicationCommandHandler(_applicationRepository.Object);
    }

    [Fact]
    public async Task Handle_ApplicationNotSubmitted_AwardingOrganisationUser_DeletesApplication()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Submitted = false
        };

        var request = new DeleteApplicationCommand(ApplicationId) { UserType = UserType.AwardingOrganisation.ToString() };

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        var result = await _handler.Handle(request, default);

        Assert.Multiple(() =>
        {
            Assert.True(result.Success);

            _applicationRepository.Verify(r => r.DeleteAsync(application), Times.Once);
        });
    }

    [Fact]
    public async Task Handle_ApplicationSubmitted_AwardingOrganisationUser_ReturnsError_AndDoesNotDelete()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Submitted = true
        };

        var request = new DeleteApplicationCommand(ApplicationId) { UserType = UserType.AwardingOrganisation.ToString() };

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        var result = await _handler.Handle(request, default);

        Assert.Multiple(() =>
        {
            Assert.False(result.Success);
            Assert.NotNull(result.InnerException);
            Assert.IsType<InvalidOperationException>(result.InnerException);

            _applicationRepository.Verify(r => r.DeleteAsync(It.IsAny<Data.Entities.Application.Application>()), Times.Never);
        });
    }

    [Fact]
    public async Task Handle_ApplicationSubmitted_QfauUser_DeletesApplication()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Submitted = true
        };

        var request = new DeleteApplicationCommand(ApplicationId) { UserType = UserType.Qfau.ToString() };

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        var result = await _handler.Handle(request, default);

        Assert.Multiple(() =>
        {
            Assert.True(result.Success);

            _applicationRepository.Verify(r => r.DeleteAsync(application), Times.Once);
        });
    }

    [Fact]
    public async Task Handle_ApplicationNotSubmitted_QfauUser_DeletesApplication()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Submitted = false
        };

        var request = new DeleteApplicationCommand(ApplicationId) { UserType = UserType.Qfau.ToString() };

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        var result = await _handler.Handle(request, default);

        Assert.Multiple(() =>
        {
            Assert.True(result.Success);

            _applicationRepository.Verify(r => r.DeleteAsync(application), Times.Once);
        });
    }

    [Fact]
    public async Task Handle_ApplicationSubmitted_UserTypeNotProvided_ReturnsError_AndDoesNotDelete()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Submitted = true
        };

        var request = new DeleteApplicationCommand(ApplicationId) { UserType = null };

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        var result = await _handler.Handle(request, default);

        Assert.Multiple(() =>
        {
            Assert.False(result.Success);
            Assert.NotNull(result.InnerException);
            Assert.IsType<InvalidOperationException>(result.InnerException);

            _applicationRepository.Verify(r => r.DeleteAsync(It.IsAny<Data.Entities.Application.Application>()), Times.Never);
        });
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ReturnsError_AndPopulatesExceptionDetails()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Submitted = false
        };

        var request = new DeleteApplicationCommand(ApplicationId) { UserType = UserType.AwardingOrganisation.ToString() };

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        _applicationRepository
            .Setup(r => r.DeleteAsync(application))
            .ThrowsAsync(new Exception(ExceptionMessage));

        var result = await _handler.Handle(request, default);

        Assert.Multiple(() =>
        {
            Assert.False(result.Success);
            Assert.NotNull(result.InnerException);
            Assert.Equal(ExceptionMessage, result.ErrorMessage);
        });
    }
}
