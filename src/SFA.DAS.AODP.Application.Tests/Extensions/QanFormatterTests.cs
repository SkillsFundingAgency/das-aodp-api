using SFA.DAS.AODP.Application.Extensions;

namespace SFA.DAS.AODP.Application.UnitTests.Extensions
{
    public class QanFormatterTests
    {
        [Theory]
        [InlineData("A/B/C", "ABC")]
        [InlineData("", "")]
        [InlineData(null, "")]
        public void RemoveSlashes_Works(string input, string expected)
        {
            var result = input.RemoveSlashes();
            Assert.Equal(expected, result);
        }
    }
}
