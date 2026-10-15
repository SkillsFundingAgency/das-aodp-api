using Moq;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Forms;

public class UpdateFormVersionCommandValidationTests
{
    private static bool IsValid(object o) =>
        Validator.TryValidateObject(o, new ValidationContext(o), new List<ValidationResult>(), true);

    [Fact]
    public void Valid_Input_Passes() =>
        Assert.True(IsValid(new UpdateFormVersionCommand
        {
            FormVersionId = Guid.NewGuid(),
            Name = "Funding form",
            Description = "Some text"
        }));

    [Theory]
    [InlineData("<script>alert(1)</script>", "ok")]
    [InlineData("=2+5", "ok")]
    [InlineData("Name", "a <b> tag")]
    public void Malicious_Input_Fails(string name, string description) =>
        Assert.False(IsValid(new UpdateFormVersionCommand
        {
            FormVersionId = Guid.NewGuid(),
            Name = name,
            Description = description
        }));

    [Fact]
    public void Over_Length_Name_Fails() =>
        Assert.False(IsValid(new UpdateFormVersionCommand
        {
            FormVersionId = Guid.NewGuid(),
            Name = new string('a', 201),
            Description = "ok"
        }));

    [Fact]
    public void Over_Length_Description_Fails() =>
        Assert.False(IsValid(new UpdateFormVersionCommand
        {
            FormVersionId = Guid.NewGuid(),
            Name = "Funding form",
            Description = new string('a', 4001)
        }));
}