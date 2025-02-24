using Moq;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

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

    }

    [Fact]
    public async Task Test_Inavlid_FormVersionId_Returns_Error()
    {

    }

}
