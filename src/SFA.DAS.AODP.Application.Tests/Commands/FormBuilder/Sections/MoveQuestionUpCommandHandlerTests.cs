using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Sections;

public class MoveQuestionUpCommandHandlerTests
{
    private readonly Mock<ISectionRepository> _sectionRepository = new Mock<ISectionRepository>();
    public MoveSectionUpCommandHandler _moveSectionUpCommandHandler;

    public MoveQuestionUpCommandHandlerTests()
    {
        _moveSectionUpCommandHandler = new(_sectionRepository.Object);
    }

    [Fact]
    public async Task Test_Move_Section_Order_Up()
    {
        var request = new MoveSectionUpCommand
        {
            SectionId = Guid.NewGuid()
        };
        _sectionRepository.Setup(v => v.IsSectionEditable(It.Is<Guid>(v => v == request.SectionId)))
            .Returns(Task.FromResult(true));
        _sectionRepository.Setup(v => v.MoveSectionOrderUp(It.Is<Guid>(v => v == request.SectionId)))
            .Returns(Task.FromResult(true));

        var result = await _moveSectionUpCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Move_Section_Order_Up_Throws_Returns_Error()
    {
        var request = new MoveSectionUpCommand
        {
            SectionId = Guid.NewGuid()
        };
        _sectionRepository.Setup(v => v.IsSectionEditable(It.Is<Guid>(v => v == request.SectionId)))
            .Returns(Task.FromResult(true));
        _sectionRepository.Setup(v => v.MoveSectionOrderUp(It.Is<Guid>(v => v == request.SectionId)))
            .Throws(new Exception("Test"));

        var result = await _moveSectionUpCommandHandler.Handle(request, default);

        Assert.False(result.Success);
        Assert.Equal("Test", result.ErrorMessage);
    }
}
