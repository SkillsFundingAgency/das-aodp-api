using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Question;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Sections;

public class DeleteSectionCommandHandlerTests
{
    private readonly Mock<ISectionRepository> _sectionRepository = new Mock<ISectionRepository>();
    public DeleteSectionCommandHandler _deleteFormVersionCommandHandler;

    public DeleteSectionCommandHandlerTests()
    {
        _deleteFormVersionCommandHandler = new(_sectionRepository.Object);
    }

    [Fact]
    public async Task Test_Delete_Form_Version()
    {
        var request = new DeleteSectionCommand(Guid.NewGuid());
        var section = new Section()
        {
            Id = Guid.NewGuid()
        };
        _sectionRepository.Setup(v => v.IsSectionEditable(It.Is<Guid>(v => v == request.SectionId)))
            .Returns(Task.FromResult(true));
        _sectionRepository.Setup(v => v.DeleteSection(It.Is<Guid>(v => v == request.SectionId)))
            .Returns(Task.FromResult(section));

        var result = await _deleteFormVersionCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Delete_Form_Throws_Returns_Error()
    {
        var request = new DeleteSectionCommand(Guid.NewGuid());
        _sectionRepository.Setup(v => v.IsSectionEditable(It.Is<Guid>(v => v == request.SectionId)))
            .Returns(Task.FromResult(true));
        _sectionRepository.Setup(v => v.DeleteSection(It.Is<Guid>(v => v == request.SectionId)))
            .Throws(new Exception("Test"));

        var result = await _deleteFormVersionCommandHandler.Handle(request, default);

        Assert.False(result.Success);
        Assert.Equal("Test", result.ErrorMessage);
    }
}
