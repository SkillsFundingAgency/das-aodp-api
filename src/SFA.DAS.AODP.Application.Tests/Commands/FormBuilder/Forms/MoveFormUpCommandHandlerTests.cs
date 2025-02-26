using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Forms;

public class MoveFormDownCommandHandlerTests
{
    private readonly Mock<IFormRepository> _formVersionRepository = new Mock<IFormRepository>();
    public MoveFormDownCommandHandler _moveFormDownCommandHandler;

    public MoveFormDownCommandHandlerTests()
    {
        _moveFormDownCommandHandler = new(_formVersionRepository.Object);
    }

    [Fact]
    public async Task Test_Delete_Form_Version()
    {
        var request = new MoveFormDownCommand(Guid.NewGuid());
        _formVersionRepository.Setup(v => v.MoveFormOrderDown(It.Is<Guid>(v => v == request.FormId)))
            .Returns(Task.FromResult(true));

        var result = await _moveFormDownCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Delete_Form_Throws_Returns_Error()
    {
        var request = new MoveFormDownCommand(Guid.NewGuid());
        _formVersionRepository.Setup(v => v.MoveFormOrderDown(It.Is<Guid>(v => v == request.FormId)))
            .Throws(new Exception("Test"));

        var result = await _moveFormDownCommandHandler.Handle(request, default);

        Assert.False(result.Success);
        Assert.Equal("Test", result.ErrorMessage);
    }
}
