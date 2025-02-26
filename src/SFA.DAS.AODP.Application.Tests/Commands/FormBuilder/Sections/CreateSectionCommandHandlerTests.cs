using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Question;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Sections;

public class CreateSectionCommandHandlerTests
{
    private readonly Mock<ISectionRepository> _sectionRepository = new Mock<ISectionRepository>();
    private readonly Mock<IFormVersionRepository> _formRepository = new Mock<IFormVersionRepository>();
    public CreateSectionCommandHandler _createSectionCommandHandler;

    public CreateSectionCommandHandlerTests()
    {
        _createSectionCommandHandler = new(_sectionRepository.Object, _formRepository.Object);
    }

    [Fact]
    public async Task Test_Create_Question()
    {
        var request = new CreateSectionCommand()
        {
            FormVersionId = Guid.NewGuid(),
            Title = "Test",
        };
        var newQuestion = new Section() { Id = Guid.NewGuid() };

        _formRepository.Setup(v => v.IsFormVersionEditable(It.Is<Guid>(v => v == request.FormVersionId)))
            .ReturnsAsync(true);

        _sectionRepository.Setup(v => v.GetMaxOrderByFormVersionId(It.Is<Guid>(v => v == request.FormVersionId)))
            .Returns(3);

        _sectionRepository.Setup(v => v.Create(It.Is<Section>(v => v.Title == request.Title)))
            .ReturnsAsync(newQuestion);

        var result = await _createSectionCommandHandler.Handle(request, default);

        Assert.True(result.Success);
        Assert.True(newQuestion.Id == result.Value.Id);
    }

    [Fact]
    public async Task Test_Create_Question_Throws_When_Page_Locked()
    {
        var request = new CreateSectionCommand()
        {
            FormVersionId = Guid.NewGuid(),
            Title = "Test",
        };
        var newQuestion = new Question() { Id = Guid.NewGuid() };

        _formRepository.Setup(v => v.IsFormVersionEditable(It.Is<Guid>(v => v == request.FormVersionId)))
            .ReturnsAsync(false);
        var result = await _createSectionCommandHandler.Handle(request, default);

        Assert.False(result.Success);
    }
}
