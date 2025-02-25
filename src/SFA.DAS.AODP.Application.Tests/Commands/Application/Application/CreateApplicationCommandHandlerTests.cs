using Moq;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Models.Form;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Application;

public class CreateApplicationCommandHandlerTests
{
    private readonly Mock<IApplicationRepository> _applicationRepository = new Mock<IApplicationRepository>();
    private readonly Mock<IFormVersionRepository> _formVersionRepository = new Mock<IFormVersionRepository>();
    public CreateApplicationCommandHandler _createApplicationCommandHandler;
    public CreateApplicationCommandHandlerTests()
    {
        _createApplicationCommandHandler
            = new CreateApplicationCommandHandler(_applicationRepository.Object, _formVersionRepository.Object);
    }


    [Fact]
    public async Task Test_Application_Created()
    {
        var formVersion = new FormVersion() 
        { 
            Id = Guid.NewGuid(),
            Status = FormVersionStatus.Published.ToString()
        };
        var application = new Data.Entities.Application.Application()
        {
            Id = Guid.NewGuid()
        };
        _formVersionRepository.Setup(v => v.GetFormVersionByIdAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult(formVersion));
        _applicationRepository.Setup(v => v.Create(It.IsAny<Data.Entities.Application.Application>()))
            .Returns(Task.FromResult(application));
        var request = new CreateApplicationCommand()
        {
            FormVersionId = formVersion.Id,
            Title = "Test",
            Owner = "Test",
            OrganisationId = Guid.NewGuid(),
            QualificationNumber = "Test"
        };
        var result = await _createApplicationCommandHandler.Handle(request, default);

        Assert.True(result.Success);
        Assert.True(result.Value.Id == application.Id);
    }

    [Fact]
    public async Task Test_Inavlid_FormVersion_Returns_Error()
    {
        var formVersion = new FormVersion()
        {
            Id = Guid.NewGuid(),
            Status = FormVersionStatus.Archived.ToString()
        };
        _formVersionRepository.Setup(v => v.GetFormVersionByIdAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult(formVersion));
        var request = new CreateApplicationCommand()
        {
            FormVersionId = formVersion.Id,
            Title = "Test",
            Owner = "Test",
            OrganisationId = Guid.NewGuid(),
            QualificationNumber = "Test"
        };
        var result = await _createApplicationCommandHandler.Handle(request, default);

        Assert.False(result.Success);
        Assert.IsAssignableFrom<InvalidOperationException>(result.InnerException);
    }

}
