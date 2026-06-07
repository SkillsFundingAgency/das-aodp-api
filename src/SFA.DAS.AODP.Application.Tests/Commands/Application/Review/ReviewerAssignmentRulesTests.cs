using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.UnitTests.Commands.Application.Review
{
    public class ReviewerAssignmentRulesTests 
    { 
        [Theory]
        [InlineData("Bob", "Bob", true)]
        [InlineData("Bob", "bob", true)]
        [InlineData(" Bob ", "bob", true)]
        [InlineData("Bob", "Alice", false)]
        [InlineData(null, "Bob", false)]
        [InlineData("Bob", null, false)]
        [InlineData("", "Bob", false)]
        [InlineData("Bob", "", false)] 
        public void WouldCauseConflict_ReturnsExpected(string reviewer1, string reviewer2, bool expected) 
        { 
            var result = ReviewerAssignmentRules.WouldCauseConflict(reviewer1, reviewer2); 
            Assert.Equal(expected, result); 
        } 
    }
}
