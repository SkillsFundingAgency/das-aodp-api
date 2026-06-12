using SFA.DAS.AODP.Application.Constants;
namespace SFA.DAS.AODP.Application.UnitTests.Constants
{
    public class RolloverStatusesTests
    {
        [Fact]
        public void RolloverStatuses_ShouldExposeCorrectConstants()
        {
            Assert.Equal("To Extend", RolloverStatuses.ToExtend);
            Assert.Equal("To Exclude", RolloverStatuses.ToExclude);
        }

        [Fact]
        public void RolloverStatuses_All_ShouldContainAllStatuses()
        {
            Assert.Equal(2, RolloverStatuses.All.Count);
            Assert.Contains(RolloverStatuses.ToExtend, RolloverStatuses.All);
            Assert.Contains(RolloverStatuses.ToExclude, RolloverStatuses.All);
        }

        [Fact]
        public void ToHList_ShouldFormatTwoStatusesCorrectly()
        {
            var result = RolloverStatuses.ToList();
            Assert.Equal("To Extend or To Exclude", result);
        }
    }
}
