using Moq;
using RestEase;
using SFA.DAS.AODP.Infrastructure.Extensions;
using SFA.DAS.AODP.Infrastructure.Services;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
using SFA.DAS.AODP.Models.Qualification;
using SFA.DAS.AODP.Models.Validation;
using System.Net;

namespace SFA.DAS.AODP.Infrastructure.UnitTests.Extensions;

public class CsvExtensionsTests
{
    [Theory]
    [InlineData("=1+1", "'=1+1")]
    [InlineData("+cmd", "'+cmd")]
    [InlineData("-2", "'-2")]
    [InlineData("@SUM(A1)", "'@SUM(A1)")]
    [InlineData("Normal text", "Normal text")]
    [InlineData("", "")]
    public void ToCsvSafe_Prefixes_Formula_Values(string input, string expected) =>
        Assert.Equal(expected, input.ToCsvSafe());
}