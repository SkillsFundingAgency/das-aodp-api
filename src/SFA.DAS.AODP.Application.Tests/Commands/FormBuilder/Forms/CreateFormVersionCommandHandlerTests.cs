using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Forms;

public class CreateFormVersionCommandHandlerTests
{
    private readonly Mock<IFormVersionRepository> _formVersionRepository = new Mock<IFormVersionRepository>();
    private readonly Mock<IFormRepository> _formRepository = new Mock<IFormRepository>();
    public CreateFormVersionCommandHandler _createFormVersionCommandHandler;

    public CreateFormVersionCommandHandlerTests()
    {
        _createFormVersionCommandHandler = new(_formVersionRepository.Object, _formRepository.Object);
    }

    [Fact]
    public async Task Test_Form_Version_Created()
    {
        var formVersion = new FormVersion()
        { 
            Id = Guid.NewGuid()
        };
        var request = new CreateFormVersionCommand()
        {
            Title = "Test",
            Description = "FooBar"
        };

        _formRepository.Setup(v => v.GetMaxOrder()).Returns(3);
        _formVersionRepository.Setup(v => v.Create(
            It.Is<FormVersion>(v => v.Title == request.Title && v.Description == request.Description),
            It.Is<int>(v => v == 4)))
            .Returns(Task.FromResult(formVersion));

        var result = await _createFormVersionCommandHandler.Handle(request, default);

        Assert.True(result.Success);
        Assert.True(result.Value.Id == formVersion.Id);
    }

    [Fact]
    public async Task Test_Form_Version_Create_Throws_Returns_Error()
    {
        var formVersion = new FormVersion()
        {
            Id = Guid.NewGuid()
        };
        var request = new CreateFormVersionCommand()
        {
            Title = "Test",
            Description = "FooBar"
        };

        _formRepository.Setup(v => v.GetMaxOrder()).Returns(3);
        _formVersionRepository.Setup(v => v.Create(
            It.Is<FormVersion>(v => v.Title == request.Title && v.Description == request.Description),
            It.Is<int>(v => v == 4)))
            .Throws(new Exception("Test"));

        var result = await _createFormVersionCommandHandler.Handle(request, default);

        Assert.False(result.Success);
        Assert.IsAssignableFrom<Exception>(result.InnerException);
        Assert.Equal("Test", result.ErrorMessage);
    }
}
