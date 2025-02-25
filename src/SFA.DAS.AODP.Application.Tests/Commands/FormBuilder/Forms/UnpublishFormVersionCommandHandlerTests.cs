using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Forms;

public class UnpublishFormVersionCommandHandlerTests
{
    private readonly Mock<IFormVersionRepository> _formVersionRepository = new Mock<IFormVersionRepository>();
    public UnpublishFormVersionCommandHandler _unpublishFormVersionCommandHandlerTests;

    public UnpublishFormVersionCommandHandlerTests()
    {
        _unpublishFormVersionCommandHandlerTests = new(_formVersionRepository.Object);
    }

    [Fact]
    public async Task Test_Delete_Form_Version()
    {
        var request = new UnpublishFormVersionCommand(Guid.NewGuid());
        _formVersionRepository.Setup(v => v.Unpublish(It.Is<Guid>(v => v == request.FormVersionId)))
            .Returns(Task.FromResult(true));

        var result = await _unpublishFormVersionCommandHandlerTests.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Delete_Form_Throws_Returns_Error()
    {
        var request = new UnpublishFormVersionCommand(Guid.NewGuid());
        _formVersionRepository.Setup(v => v.Unpublish(It.Is<Guid>(v => v == request.FormVersionId)))
            .Throws(new Exception("Test"));

        var result = await _unpublishFormVersionCommandHandlerTests.Handle(request, default);

        Assert.False(result.Success);
        Assert.Equal("Test", result.ErrorMessage);
    }
}
