using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Question;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Pages;

public class UpdatePageCommandHandlerTests
{
    public Mock<IPageRepository> _sectionRepository = new Mock<IPageRepository>();
    public UpdatePageCommandHandler _updatePageCommandHandler;

    public UpdatePageCommandHandlerTests()
    {
        _updatePageCommandHandler = new(_sectionRepository.Object);
    }

    [Fact]
    public async Task Test_Update_Section()
    {
        var request = new UpdatePageCommand()
        {
            Id = Guid.NewGuid(),
            FormVersionId = Guid.NewGuid(),
            Title = "Test"
        };

        var newQuestion = new Page() 
        { 
            Id = Guid.NewGuid() ,
            Title = "",
        };

        _sectionRepository.Setup(v => v.IsPageEditable(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(true);

        _sectionRepository.Setup(v => v.GetPageByIdAsync(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(newQuestion);

        _sectionRepository.Setup(v => v.Update(It.Is<Page>(v => v.Title == request.Title)))
            .ReturnsAsync(newQuestion);

        var result = await _updatePageCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Create_Section_Throws_When_Section_Locked()
    {
        var request = new UpdatePageCommand()
        {
            Id = Guid.NewGuid(),
            FormVersionId = Guid.NewGuid(),
            Title = "Test"
        };

        var newQuestion = new Page()
        {
            Id = Guid.NewGuid(),
            Title = "",
        };

        _sectionRepository.Setup(v => v.IsPageEditable(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(false);

        _sectionRepository.Setup(v => v.GetPageByIdAsync(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(newQuestion);

        _sectionRepository.Setup(v => v.Update(It.Is<Page>(v => v.Title == request.Title)))
            .ReturnsAsync(newQuestion);

        var result = await _updatePageCommandHandler.Handle(request, default);

        Assert.False(result.Success);
    }
}
