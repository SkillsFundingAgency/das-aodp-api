using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Question;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Pages;

public class DeletePageCommandHandlerTests
{
    private readonly Mock<IPageRepository> _pageRepository = new Mock<IPageRepository>();
    public DeletePageCommandHandler _deletePageCommandHandler;

    public DeletePageCommandHandlerTests()
    {
        _deletePageCommandHandler = new(_pageRepository.Object);
    }

    [Fact]
    public async Task Test_Delete_Form_Version()
    {
        var request = new DeletePageCommand(Guid.NewGuid());
        var section = new Page()
        {
            Id = Guid.NewGuid()
        };
        _pageRepository.Setup(v => v.IsPageEditable(It.Is<Guid>(v => v == request.PageId)))
            .Returns(Task.FromResult(true));
        _pageRepository.Setup(v => v.Archive(It.Is<Guid>(v => v == request.PageId)))
            .Returns(Task.FromResult<Page?>(section));

        var result = await _deletePageCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Delete_Form_Throws_Returns_Error()
    {
        var request = new DeletePageCommand(Guid.NewGuid());
        _pageRepository.Setup(v => v.IsPageEditable(It.Is<Guid>(v => v == request.PageId)))
            .Returns(Task.FromResult(true));
        _pageRepository.Setup(v => v.Archive(It.Is<Guid>(v => v == request.PageId)))
            .Throws(new Exception("Test"));

        var result = await _deletePageCommandHandler.Handle(request, default);

        Assert.False(result.Success);
        Assert.Equal("Test", result.ErrorMessage);
    }
}
