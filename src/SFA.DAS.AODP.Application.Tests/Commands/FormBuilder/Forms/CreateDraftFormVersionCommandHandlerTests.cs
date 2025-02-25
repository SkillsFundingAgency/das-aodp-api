using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using Moq;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Forms;

public class UpdateFormVersionCommandHandlerTest
{
    private readonly Mock<IFormVersionRepository> _formVersionRepository = new Mock<IFormVersionRepository>();
    public UpdateFormVersionCommandHandler _updateFormVersionCommandHandler;

    public UpdateFormVersionCommandHandlerTest()
    {
        _updateFormVersionCommandHandler = new(_formVersionRepository.Object);
    }

    [Fact]
    public async Task Test_Create_Draft_Form_Version()
    {
        var formVersion = new FormVersion() 
        { 
            Id = Guid.NewGuid()
        };
        var request = new UpdateFormVersionCommand() 
        {
            FormVersionId = formVersion.Id,
            Name = "Test",
            Description = "Test"
        };
        _formVersionRepository.Setup(v => v.IsFormVersionEditable(It.Is<Guid>(v => v == request.FormVersionId)))
            .Returns(Task.FromResult(true));

        _formVersionRepository.Setup(v => v.GetFormVersionByIdAsync(It.Is<Guid>(v => v == request.FormVersionId)))
            .Returns(Task.FromResult<FormVersion>(formVersion));

        _formVersionRepository.Setup(v => v.Update(It.Is<FormVersion>(v => v.Title == request.Name && v.Description == request.Description)))
            .Returns(Task.FromResult<FormVersion>(formVersion));

        var result = await _updateFormVersionCommandHandler.Handle(request, default);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Create_Draft_Form_Throws_When_Record_Locked()
    {
        var formVersion = new FormVersion()
        {
            Id = Guid.NewGuid()
        };
        var request = new UpdateFormVersionCommand()
        {
            FormVersionId = Guid.NewGuid(),
            Name = "Test",
            Description = "Test"
        };
        _formVersionRepository.Setup(v => v.IsFormVersionEditable(It.Is<Guid>(v => v == request.FormVersionId)))
            .Returns(Task.FromResult(false));

        var result = await _updateFormVersionCommandHandler.Handle(request, default);

        Assert.False(result.Success);
    }
}
