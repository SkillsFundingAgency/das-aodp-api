using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using Moq;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Forms;

public class CreateDraftFormVersionCommandHandlerTests
{
    private readonly Mock<IFormVersionRepository> _formVersionRepository = new Mock<IFormVersionRepository>();
    public CreateDraftFormVersionCommandHandler _createDraftFormVersionCommandHandler;

    public CreateDraftFormVersionCommandHandlerTests()
    {
        _createDraftFormVersionCommandHandler = new(_formVersionRepository.Object);
    }

    [Fact]
    public async Task Test_Create_Draft_Form_Version()
    {
        var formVersion = new FormVersion() { Id = Guid.NewGuid() };
        var newFormVersion = new FormVersion() { Id = Guid.NewGuid() };
        var request = new CreateDraftFormVersionCommand(Guid.NewGuid());
        _formVersionRepository.Setup(v => v.GetDraftFormVersionByFormId(It.Is<Guid>(v => v == request.FormId)))
            .Returns(Task.FromResult<FormVersion?>(null));

        _formVersionRepository.Setup(v => v.GetPublishedFormVersionByFormId(It.Is<Guid>(v => v == request.FormId)))
            .Returns(Task.FromResult<FormVersion?>(formVersion));
        _formVersionRepository.Setup(v => v.CreateDraftAsync(It.Is<Guid>(v => v == formVersion.Id)))
            .Returns(Task.FromResult<FormVersion>(newFormVersion));

        var result = await _createDraftFormVersionCommandHandler.Handle(request, default);

        Assert.True(result.Success);
        Assert.Equal(result.Value.FormVersionId, newFormVersion.Id);
    }

    [Fact]
    public async Task Test_Create_Draft_Form_Throws_When_Draft_Exists()
    {
        var formVersion = new FormVersion() { Id = Guid.NewGuid() };
        var request = new CreateDraftFormVersionCommand(Guid.NewGuid());

        _formVersionRepository.Setup(v => v.GetDraftFormVersionByFormId(It.Is<Guid>(v => v == request.FormId)))
            .Returns(Task.FromResult<FormVersion?>(formVersion));

        var result = await _createDraftFormVersionCommandHandler.Handle(request, default);

        Assert.False(result.Success);
        Assert.Equal("A draft version already exists", result.ErrorMessage);
    }

    [Fact]
    public async Task Test_Create_Draft_Form_Throws_When_No_Published_Version()
    {
        var request = new CreateDraftFormVersionCommand(Guid.NewGuid());
        _formVersionRepository.Setup(v => v.GetDraftFormVersionByFormId(It.Is<Guid>(v => v == request.FormId)))
            .Returns(Task.FromResult<FormVersion?>(null));

        _formVersionRepository.Setup(v => v.GetPublishedFormVersionByFormId(It.Is<Guid>(v => v == request.FormId)))
            .Returns(Task.FromResult<FormVersion?>(null));

        var result = await _createDraftFormVersionCommandHandler.Handle(request, default);

        Assert.False(result.Success);
        Assert.Equal("No published version to clone", result.ErrorMessage);
    }
}
