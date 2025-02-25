using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Forms;

public class PublishFormVersionCommandHandlerTests
{
    private readonly Mock<IFormVersionRepository> _formVersionRepository = new Mock<IFormVersionRepository>();
    public PublishFormVersionCommandHandler _deleteFormVersionCommandHandler;

    public PublishFormVersionCommandHandlerTests()
    {
        _deleteFormVersionCommandHandler = new(_formVersionRepository.Object);
    }

    [Fact]
    public async Task Test_Publish_Form_Version()
    {
        var request = new PublishFormVersionCommand(Guid.NewGuid());
        _formVersionRepository.Setup(v => v.Publish(It.Is<Guid>(v => v == request.FormVersionId)))
            .Returns(Task.FromResult(true));

        var result = await _deleteFormVersionCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Publish_Form_Version_Throws_Returns_Error()
    {
        var request = new PublishFormVersionCommand(Guid.NewGuid());
        _formVersionRepository.Setup(v => v.Publish(It.Is<Guid>(v => v == request.FormVersionId)))
            .Throws(new Exception("Test"));

        var result = await _deleteFormVersionCommandHandler.Handle(request, default);

        Assert.False(result.Success);
        Assert.Equal("Test", result.ErrorMessage);
    }
}
