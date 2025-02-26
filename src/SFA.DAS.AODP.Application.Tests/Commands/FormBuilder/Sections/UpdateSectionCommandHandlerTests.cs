using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Question;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Sections;

public class UpdateSectionCommandHandlerTests
{
    public Mock<ISectionRepository> _sectionRepository = new Mock<ISectionRepository>();
    public UpdateSectionCommandHandler _updateSectionCommandHandler;

    public UpdateSectionCommandHandlerTests()
    {
        _updateSectionCommandHandler = new(_sectionRepository.Object);
    }

    [Fact]
    public async Task Test_Update_Section()
    {
        var request = new UpdateSectionCommand()
        {
            Id = Guid.NewGuid(),
            FormVersionId = Guid.NewGuid(),
            Title = "Test"
        };

        var newQuestion = new Section() 
        { 
            Id = Guid.NewGuid() ,
            Title = "",
        };

        _sectionRepository.Setup(v => v.IsSectionEditable(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(true);

        _sectionRepository.Setup(v => v.GetSectionByIdAsync(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(newQuestion);

        _sectionRepository.Setup(v => v.Update(It.Is<Section>(v => v.Title == request.Title)))
            .ReturnsAsync(newQuestion);

        var result = await _updateSectionCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Create_Section_Throws_When_Section_Locked()
    {
        var request = new UpdateSectionCommand()
        {
            Id = Guid.NewGuid(),
            FormVersionId = Guid.NewGuid(),
            Title = "Test"
        };

        var newQuestion = new Section()
        {
            Id = Guid.NewGuid(),
            Title = "",
        };

        _sectionRepository.Setup(v => v.IsSectionEditable(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(false);

        _sectionRepository.Setup(v => v.GetSectionByIdAsync(It.Is<Guid>(v => v == request.Id)))
            .ReturnsAsync(newQuestion);

        _sectionRepository.Setup(v => v.Update(It.Is<Section>(v => v.Title == request.Title)))
            .ReturnsAsync(newQuestion);

        var result = await _updateSectionCommandHandler.Handle(request, default);

        Assert.False(result.Success);
    }
}
