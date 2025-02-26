using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Question;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Pages;

public class MovePageDownCommandHandlerTests
{
    private readonly Mock<IPageRepository> _pageRepository = new Mock<IPageRepository>();
    public MovePageDownCommandHandler _movePageDownCommandHandler;

    public MovePageDownCommandHandlerTests()
    {
        _movePageDownCommandHandler = new(_pageRepository.Object);
    }

    [Fact]
    public async Task Test_Delete_Form_Version()
    {
        var request = new MovePageDownCommand() 
        {
            PageId = Guid.NewGuid()
        };
        var section = new Data.Entities.FormBuilder.Page()
        {
            Id = Guid.NewGuid()
        };
        _pageRepository.Setup(v => v.IsPageEditable(It.Is<Guid>(v => v == request.PageId)))
            .Returns(Task.FromResult(true));
        _pageRepository.Setup(v => v.MovePageOrderDown(It.Is<Guid>(v => v == request.PageId)))
            .Returns(Task.FromResult(true));

        var result = await _movePageDownCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Delete_Form_Throws_Returns_Error()
    {
        var request = new MovePageDownCommand()
        {
            PageId = Guid.NewGuid()
        };
        _pageRepository.Setup(v => v.IsPageEditable(It.Is<Guid>(v => v == request.PageId)))
            .Returns(Task.FromResult(true));
        _pageRepository.Setup(v => v.MovePageOrderDown(It.Is<Guid>(v => v == request.PageId)))
            .Throws(new Exception("Test"));

        var result = await _movePageDownCommandHandler.Handle(request, default);

        Assert.False(result.Success);
        Assert.Equal("Test", result.ErrorMessage);
    }
}
