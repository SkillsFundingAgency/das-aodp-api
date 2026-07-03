using SFA.DAS.AODP.Application.Constants;
using SFA.DAS.AODP.Models.Rollover;
using Xunit;

namespace SFA.DAS.AODP.Application.UnitTests.Constants
{
    public class RolloverStatusInfoTests
    {

        [Fact]
        public void FromCsv_ReturnsUnknown_WhenNull()
        {
            var result = RolloverStatusInfo.FromCsv(null);

            Assert.Equal(RolloverStatus.Unknown, result);
        }

        [Fact]
        public void FromCsv_ParsesExtended_IgnoringCaseAndWhitespace()
        {
            var result = RolloverStatusInfo.FromCsv("  ExTeNdEd ");

            Assert.Equal(RolloverStatus.Extended, result);
        }

        [Fact]
        public void FromCsv_ParsesExcluded()
        {
            var result = RolloverStatusInfo.FromCsv("excluded");

            Assert.Equal(RolloverStatus.Excluded, result);
        }

        [Fact]
        public void FromCsv_ParsesNeedsReview()
        {
            var result = RolloverStatusInfo.FromCsv("NeedsReview");

            Assert.Equal(RolloverStatus.NeedsReview, result);
        }

        [Fact]
        public void FromCsv_ReturnsUnknown_ForInvalidValue()
        {
            var result = RolloverStatusInfo.FromCsv("not-a-status");

            Assert.Equal(RolloverStatus.Unknown, result);
        }


        [Fact]
        public void ToDisplay_ReturnsExtended()
        {
            var result = RolloverStatus.Extended.ToDisplay();

            Assert.Equal("Extended", result);
        }

        [Fact]
        public void ToDisplay_ReturnsExcluded()
        {
            var result = RolloverStatus.Excluded.ToDisplay();

            Assert.Equal("Excluded", result);
        }

        [Fact]
        public void ToDisplay_ReturnsNeedsReview()
        {
            var result = RolloverStatus.NeedsReview.ToDisplay();

            Assert.Equal("Needs Review", result);
        }

        [Fact]
        public void ToDisplay_ReturnsEnumName_ForUnknownValues()
        {
            var result = RolloverStatus.Unknown.ToDisplay();

            Assert.Equal("Unknown", result);
        }
    }
}
