using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Question;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Pages;

public class CreatePageCommandHandlerTests
{
    private readonly Mock<ISectionRepository> _sectionRepository = new Mock<ISectionRepository>();
    private readonly Mock<IPageRepository> _pageRepository = new Mock<IPageRepository>();
    public CreatePageCommandHandler _createPageCommandHandler;

    public CreatePageCommandHandlerTests()
    {
        _createPageCommandHandler = new(_pageRepository.Object, _sectionRepository.Object);
    }

    [Fact]
    public async Task Test_Create_Page()
    {
        var request = new CreatePageCommand()
        {
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            Title = "Test",
        };
        var newQuestion = new Page() { Id = Guid.NewGuid() };

        _sectionRepository.Setup(v => v.IsSectionEditable(It.Is<Guid>(v => v == request.SectionId)))
            .ReturnsAsync(true);

        _pageRepository.Setup(v => v.GetMaxOrderBySectionId(It.Is<Guid>(v => v == request.SectionId)))
            .Returns(3);

        _pageRepository.Setup(v => v.Create(It.Is<Page>(v => v.Title == request.Title)))
            .ReturnsAsync(newQuestion);

        var result = await _createPageCommandHandler.Handle(request, default);

        Assert.True(result.Success);
        Assert.True(newQuestion.Id == result.Value.Id);
    }

    [Fact]
    public async Task Test_Page_Question_Throws_When_Section_Locked()
    {
        var request = new CreatePageCommand()
        {
            FormVersionId = Guid.NewGuid(),
            SectionId = Guid.NewGuid(),
            Title = "Test",
        };
        var newQuestion = new Page() { Id = Guid.NewGuid() };

        _sectionRepository.Setup(v => v.IsSectionEditable(It.Is<Guid>(v => v == request.SectionId)))
            .ReturnsAsync(false);
        var result = await _createPageCommandHandler.Handle(request, default);

        Assert.False(result.Success);
    }
}
