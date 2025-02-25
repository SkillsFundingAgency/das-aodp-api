using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Pages;

public class MovePageUpCommandHandlerTests
{
    private readonly Mock<IPageRepository> _pageRepository = new Mock<IPageRepository>();
    public MovePageUpCommandHandler _movePageUpCommandHandler;

    public MovePageUpCommandHandlerTests()
    {
        _movePageUpCommandHandler = new(_pageRepository.Object);
    }

    [Fact]
    public async Task Test_Move_Page_Up()
    {
        var request = new MovePageUpCommand()
        {
            PageId = Guid.NewGuid()
        };
        var section = new Data.Entities.FormBuilder.Page()
        {
            Id = Guid.NewGuid()
        };
        _pageRepository.Setup(v => v.IsPageEditable(It.Is<Guid>(v => v == request.PageId)))
            .Returns(Task.FromResult(true));
        _pageRepository.Setup(v => v.MovePageOrderUp(It.Is<Guid>(v => v == request.PageId)))
            .Returns(Task.FromResult(true));

        var result = await _movePageUpCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Move_Page_Up_Throws_Returns_Error()
    {
        var request = new MovePageUpCommand()
        {
            PageId = Guid.NewGuid()
        };
        _pageRepository.Setup(v => v.IsPageEditable(It.Is<Guid>(v => v == request.PageId)))
            .Returns(Task.FromResult(true));
        _pageRepository.Setup(v => v.MovePageOrderUp(It.Is<Guid>(v => v == request.PageId)))
            .Throws(new Exception("Test"));

        var result = await _movePageUpCommandHandler.Handle(request, default);

        Assert.False(result.Success);
        Assert.Equal("Test", result.ErrorMessage);
    }
}
