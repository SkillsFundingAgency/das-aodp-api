using Moq;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Application;
public class EditApplicationCommandHandlerTests
{
    private readonly Mock<IApplicationRepository> _applicationRepository = new Mock<IApplicationRepository>();
    public EditApplicationCommandHandler _editApplicationCommandHandler;

    public EditApplicationCommandHandlerTests()
    {
        _editApplicationCommandHandler = new(_applicationRepository.Object);
    }

    [Fact]
    public async Task Test_Application_Updated()
    {
        var application = new Data.Entities.Application.Application()
        {
            Submitted = false,
            Name = "",
            QualificationNumber = "",
            Owner = ""
        };
        var request = new EditApplicationCommand()
        {
            ApplicationId = Guid.NewGuid(),
            QualificationNumber = "Test",
            Title = "Test",
            Owner = "Test",
        };
        _applicationRepository.Setup(v => v.GetByIdAsync(It.Is<Guid>(v => v == request.ApplicationId)))
            .Returns(Task.FromResult(application));
        _applicationRepository.Setup(v => v.UpdateAsync(It.Is<Data.Entities.Application.Application>(v =>
                v.Name == request.Title && 
                    v.Owner == request.Owner && 
                    v.QualificationNumber == request.QualificationNumber
            )))
            .Returns(Task.CompletedTask);

        var result = await _editApplicationCommandHandler.Handle(request, default);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Application_Locked_Returns_Error()
    {
        var application = new Data.Entities.Application.Application()
        {
            Submitted = true,
            Name = "",
            QualificationNumber = "",
            Owner = ""
        };
        var request = new EditApplicationCommand()
        {
            ApplicationId = Guid.NewGuid(),
            QualificationNumber = "Test",
            Title = "Test",
            Owner = "Test",
        };
        _applicationRepository.Setup(v => v.GetByIdAsync(It.Is<Guid>(v => v == request.ApplicationId)))
            .Returns(Task.FromResult(application));
        _applicationRepository.Setup(v => v.UpdateAsync(It.Is<Data.Entities.Application.Application>(v =>
                v.Name == request.Title &&
                    v.Owner == request.Owner &&
                    v.QualificationNumber == request.QualificationNumber
            )))
            .Returns(Task.CompletedTask);

        var result = await _editApplicationCommandHandler.Handle(request, default);
        Assert.False(result.Success);
        Assert.IsAssignableFrom<RecordLockedException>(result.InnerException);
    }
}
