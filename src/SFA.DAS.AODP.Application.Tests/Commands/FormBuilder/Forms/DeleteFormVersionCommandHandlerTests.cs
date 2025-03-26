using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Forms;

public class DeleteFormVersionCommandHandlerTests
{
    private readonly Mock<IFormRepository> _formRepository = new Mock<IFormRepository>();
    public DeleteFormCommandHandler _deleteFormVersionCommandHandler;

    public DeleteFormVersionCommandHandlerTests()
    {
        _deleteFormVersionCommandHandler = new(_formRepository.Object);
    }

    [Fact]
    public async Task Test_Delete_Form_Version()
    {
        var request = new DeleteFormCommand(Guid.NewGuid());
        _formRepository.Setup(v => v.Archive(It.Is<Guid>(v => v == request.FormId)))
            .Returns(Task.FromResult(true));

        var result = await _deleteFormVersionCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Delete_Form_Throws_Returns_Error()
    {
        var request = new DeleteFormCommand(Guid.NewGuid());
        _formRepository.Setup(v => v.Archive(It.Is<Guid>(v => v == request.FormId)))
            .Throws(new Exception("Test"));

        var result = await _deleteFormVersionCommandHandler.Handle(request, default);

        Assert.False(result.Success);
        Assert.Equal("Test", result.ErrorMessage);
    }
}
