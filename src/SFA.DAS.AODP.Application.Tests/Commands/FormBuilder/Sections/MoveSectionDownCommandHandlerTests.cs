using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Question;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Sections;

public class MoveSectionDownCommandHandlerTests
{
    private readonly Mock<ISectionRepository> _sectionRepository = new Mock<ISectionRepository>();
    public MoveSectionDownCommandHandler _moveSectionDownCommandHandler;

    public MoveSectionDownCommandHandlerTests()
    {
        _moveSectionDownCommandHandler = new(_sectionRepository.Object);
    }

    [Fact]
    public async Task Test_Move_Question_Order_Down()
    {
        var request = new MoveSectionDownCommand
        {
            SectionId = Guid.NewGuid()
        };
        _sectionRepository.Setup(v => v.IsSectionEditable(It.Is<Guid>(v => v == request.SectionId)))
            .Returns(Task.FromResult(true));
        _sectionRepository.Setup(v => v.MoveSectionOrderDown(It.Is<Guid>(v => v == request.SectionId)))
            .Returns(Task.FromResult(true));

        var result = await _moveSectionDownCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Move_Question_Order_Down_Throws_Returns_Error()
    {
        var request = new MoveSectionDownCommand
        {
            SectionId = Guid.NewGuid()
        };
        _sectionRepository.Setup(v => v.IsSectionEditable(It.Is<Guid>(v => v == request.SectionId)))
            .Returns(Task.FromResult(true));
        _sectionRepository.Setup(v => v.MoveSectionOrderDown(It.Is<Guid>(v => v == request.SectionId)))
            .Throws(new Exception("Test"));

        var result = await _moveSectionDownCommandHandler.Handle(request, default);

        Assert.False(result.Success);
        Assert.Equal("Test", result.ErrorMessage);
    }
}
