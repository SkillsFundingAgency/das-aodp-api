using System.ComponentModel.DataAnnotations;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

namespace SFA.DAS.AODP.Application.Tests.Commands.FormBuilder.Forms;

public class CreateFormVersionCommandValidationTests
{
    private static bool IsValid(object o) =>
        Validator.TryValidateObject(o, new ValidationContext(o), new List<ValidationResult>(), true);

    [Fact]
    public void Valid_Input_Passes() =>
        Assert.True(IsValid(new CreateFormVersionCommand { Title = "Funding form", Description = "Some text" }));

    [Theory]
    [InlineData("<script>alert(1)</script>", "ok")]
    [InlineData("=2+5", "ok")]
    [InlineData("Title", "a <b> tag")]
    public void Malicious_Input_Fails(string title, string description) =>
        Assert.False(IsValid(new CreateFormVersionCommand { Title = title, Description = description }));

    [Fact]
    public void Over_Length_Title_Fails() =>
        Assert.False(IsValid(new CreateFormVersionCommand { Title = new string('a', 201), Description = "ok" }));
}