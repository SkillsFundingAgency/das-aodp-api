using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Forms;

public class DeleteFormVersionCommandHandlerTests
{
    private readonly Mock<IFormVersionRepository> _formVersionRepository = new Mock<IFormVersionRepository>();
    public DeleteFormVersionCommandHandler _deleteFormVersionCommandHandler;

    public DeleteFormVersionCommandHandlerTests()
    {
        _deleteFormVersionCommandHandler = new(_formVersionRepository.Object);
    }

    [Fact]
    public async Task Test_Delete_Form_Version()
    {
        var request = new DeleteFormVersionCommand(Guid.NewGuid());
        _formVersionRepository.Setup(v => v.Archive(It.Is<Guid>(v => v == request.FormVersionId)))
            .Returns(Task.FromResult(true));

        var result = await _deleteFormVersionCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Delete_Form_Throws_Returns_Error()
    {
        var request = new DeleteFormVersionCommand(Guid.NewGuid());
        _formVersionRepository.Setup(v => v.Archive(It.Is<Guid>(v => v == request.FormVersionId)))
            .Throws(new Exception("Test"));

        var result = await _deleteFormVersionCommandHandler.Handle(request, default);

        Assert.False(result.Success);
        Assert.Equal("Test", result.ErrorMessage);
    }
}
